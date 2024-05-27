using LibApp.Models;
using LibApp.Repositories;
using LibApp.ViewModels.SubViewModels;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class OrdersHistoryTabVM : BaseVM
    {
        readonly Login _user;

        long? _orderNumberToSearch = null;
        DateTime? _orderDateToSearch = null;

        decimal _currOrderCost = 0;

        int _ordersListOffset = 0;
        bool _nextOrdersPageIsAvailable;
        bool _prevOrdersPageIsAvailable = false;

        OrderVM _currOrder = new(new() 
        { 
            Id = 0,
            Date = DateTime.Now,
            Name = string.Empty,
            Email = string.Empty,
            Address = string.Empty,
            Comment = string.Empty,
            IdStatusNavigation = null!,
        });

        int _orderBooksListOffset = 0;
        bool _nextPageIsAvailable;
        bool _prevPageIsAvailable = false;

        ExpressionStarter<Order> _totalFilter = null!;

        ExpressionStarter<Order> _orderNumberFilter;
        ExpressionStarter<Order> _orderDateFilter;

        public ObservableCollection<Order> OrdersList { get; set; }
        public ObservableCollection<OrderedBook> OrderBooks { get; set; }
        public ReadOnlyCollection<OrderStatus> OrderStatuses { get; init; }

        public long? OrderNumberToSearch
        {
            get => _orderNumberToSearch;
            set
            {
                _orderNumberToSearch = value;
                OnPropertyChanged(nameof(OrderNumberToSearch));
                if (_orderNumberToSearch is null)
                {
                    _orderNumberFilter = PredicateBuilder.New<Order>(true);
                }
                else
                {
                    _orderNumberFilter = PredicateBuilder.New<Order>()
                        .Or(o => o.Id == _orderNumberToSearch);
                }
                UpdateFilterAndOrdersList();
            }
        }
        public DateTime? OrderDateToSearch
        {
            get => _orderDateToSearch;
            set
            {
                _orderDateToSearch = value;
                OnPropertyChanged(nameof(OrderDateToSearch));
                if (_orderDateToSearch is null)
                {
                    _orderDateFilter = PredicateBuilder.New<Order>(true);
                }
                else
                {
                    _orderDateFilter = PredicateBuilder.New<Order>()
                        .Or(o => o.Date.Date == _orderDateToSearch.Value.Date);
                }
                UpdateFilterAndOrdersList();
            }
        }
        public OrderVM CurrOrder
        {
            get => _currOrder;
            set { _currOrder = value; OnPropertyChanged(nameof(CurrOrder)); }
        }

        public bool NextOrdersPageIsAvailable
        {
            get => _nextOrdersPageIsAvailable;
            set
            {
                _nextOrdersPageIsAvailable = value;
                OnPropertyChanged(nameof(NextOrdersPageIsAvailable));
            }
        }
        public bool PrevOrdersPageIsAvailable
        {
            get => _prevOrdersPageIsAvailable;
            set
            {
                _prevOrdersPageIsAvailable = value;
                OnPropertyChanged(nameof(PrevOrdersPageIsAvailable));
            }
        }

        public bool NextPageIsAvailable
        {
            get => _nextPageIsAvailable;
            set
            {
                _nextPageIsAvailable = value;
                OnPropertyChanged(nameof(NextPageIsAvailable));
            }
        }
        public bool PrevPageIsAvailable
        {
            get => _prevPageIsAvailable;
            set
            {
                _prevPageIsAvailable = value;
                OnPropertyChanged(nameof(PrevPageIsAvailable));
            }
        }

        public long UserId { get => _user.Id; set => _ = value; }
        public bool IsAdmin { get; set; }
        public bool IsNotAdmin => !IsAdmin;

        public bool OrdersListIsNotEmpty => OrdersList?.Any() ?? false;
        public bool OrdersListIsEmpty => !OrdersListIsNotEmpty;

        public decimal CurrOrderCost
        {
            get => _currOrderCost;
            set
            {
                _currOrderCost = value;
                OnPropertyChanged(nameof(CurrOrderCost));
            }
        }
        public bool ContentIsNotEmpty => OrderBooks?.Any() ?? false;
        public bool ContentIsEmpty => !ContentIsNotEmpty;

        public OrdersHistoryTabVM(Login user)
        {
            _user = user;
            IsAdmin = user.IdRoleNavigation.Name == Constants.AdministratorRoleName;

            OrdersList = new ObservableCollection<Order>();
            OrderBooks = new ObservableCollection<OrderedBook>();
            using var db = new DbUnit();
            OrderStatuses = db.OrderStatuses.GetAll().ToArray().AsReadOnly();
            OnPropertyChanged(nameof(OrderStatuses));

            VMInteractionsManager.BooksUpdated += () => UpdateOrderBooksContent();
            VMInteractionsManager.OrdersUpdated += () =>
            {
                UpdateFilterAndOrdersList();
                UpdateOrderBooksContent();
            };

            OrdersListOnSelectionChangedCmd = new(ExecuteOrdersListOnSelectionChangedCmd);
            OrderBooksOnSelectionChangedCmd = new(ExecuteOrderBooksOnSelectionChangedCmd);
            OrderStatusOnSelectionChangedCmd = new(ExecuteOrderStatusOnSelectionChangedCmd);
            NextOrdersPageCmd = new(ExecuteNextOrdersPageCmd);
            PrevOrdersPageCmd = new(ExecutePrevOrdersPageCmd);
            NextPageCmd = new(ExecuteNextPageCmd);
            PrevPageCmd = new(ExecutePrevPageCmd);

            _orderNumberFilter = PredicateBuilder.New<Order>(true);
            _orderDateFilter = PredicateBuilder.New<Order>(true);

            UpdateFilterAndOrdersList();
            UpdateOrderBooksContent();
        }

        public RelayCommand OrdersListOnSelectionChangedCmd { get; init; }
        public RelayCommand OrderBooksOnSelectionChangedCmd { get; init; }
        public RelayCommand OrderStatusOnSelectionChangedCmd { get; init; }
        public RelayCommand NextOrdersPageCmd { get; init; }
        public RelayCommand PrevOrdersPageCmd { get; init; }
        public RelayCommand NextPageCmd { get; init; }
        public RelayCommand PrevPageCmd { get; init; }

        void ExecuteOrdersListOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ListView lv && lv.SelectedItem is Order o))
            {
                return;
            }

            SetOrderToView(o);
        }
        void ExecuteOrderBooksOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ListView lv && lv.SelectedItem is OrderedBook ob))
            {
                return;
            }

            lv.SelectedItem = null;

            if (IsAdmin)
            {
                VMInteractionsManager.OnBookToEditSelected(ob.IdBookNavigation);
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookEditTabA);
            }
            else
            {
                VMInteractionsManager.OnBookToViewSelected(ob.IdBookNavigation);
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookInfoTabU);
            }
        }
        void ExecuteOrderStatusOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ComboBox cb && cb.SelectedItem is OrderStatus os))
            {
                return;
            }

            if (_currOrder.IdStatus == os.Id)
            {
                return;
            }

            static bool osIsConfirmed(OrderStatus? os) =>  os is not null && 
                (os.Name == Constants.ShippedOrderStatusName ||
                 os.Name == Constants.DeliveredOrderStatusName);

            using var db = new DbUnit();
            if (!osIsConfirmed(_currOrder.IdStatusNavigation) && osIsConfirmed(os))
            {
                var notEnoughBooks = db.OrderedBooks.GetWhereNotEnough(_currOrder.Id);

                int nNotEnoughBooks = notEnoughBooks.Count();
                if (nNotEnoughBooks != 0)
                {
                    var confirmOrder = WPFCustomMessageBox.CustomMessageBox.ShowYesNo($"""
                        У заказе ёсць кнігі, якіх недастаткова на складзе:
                        {string.Join(", ", notEnoughBooks
                               .Take(Constants.MaxNotEnoughBookIdsDisplayedNumber)
                               .Select(ob => ob.IdBookNavigation.Title).AsEnumerable())}{(nNotEnoughBooks > Constants.MaxNotEnoughBookIdsDisplayedNumber ? "..." : string.Empty)}

                        Усё роўна пацвердзіць заказ?
                        """, "Пацвярджэнне заказа", "Пацвердзіць", "Скасаваць", MessageBoxImage.Warning);

                    if (confirmOrder == MessageBoxResult.No)
                    {
                        OnPropertyChanged(nameof(_currOrder.IdStatusNavigation));
                        return;
                    }
                }
            }

            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                Task.Run(() => db.Orders.UpdateOrderStatus(_currOrder.Id, os.Id)).Wait();
                Task.Run(db.SaveAsync).Wait();
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ProgramError.Show($"{ex}");
                return;
            }
            _currOrder.IdStatusNavigation = os;
            UpdateOrdersList(false);
        }
        void ExecuteNextOrdersPageCmd(object? obj)
        {
            _ordersListOffset += Constants.MaxOrdersNumberOnPageInOrdersList;
            UpdateOrdersList(false);
        }
        void ExecutePrevOrdersPageCmd(object? obj)
        {
            _ordersListOffset -= Constants.MaxOrdersNumberOnPageInOrdersList;
            UpdateOrdersList(false);
        }
        void ExecuteNextPageCmd(object? obj)
        {
            _orderBooksListOffset += Constants.MaxBooksNumberOnPageInOrder;
            UpdateOrderBooksContent(false);
        }
        void ExecutePrevPageCmd(object? obj)
        {
            _orderBooksListOffset -= Constants.MaxBooksNumberOnPageInOrder;
            UpdateOrderBooksContent(false);
        }
        
        void UpdateFilterAndOrdersList()
        {
            _totalFilter = PredicateBuilder.New<Order>(true)
                .And(_orderNumberFilter)
                .And(_orderDateFilter);

            UpdateOrdersList();
        }
        void UpdateOrdersList(bool clearOffset = true)
        {
            if (clearOffset)
            {
                _ordersListOffset = 0;
            }

            using var db = new DbUnit();
            IQueryable<Order> filteredOrders = (IsAdmin
                    ? db.Orders.GetAll()
                    : db.Orders.GetByUserId(_user.Id))
                .Where(_totalFilter)
                .OrderByDescending(o => o.Id);
            filteredOrders = filteredOrders.Skip(_ordersListOffset);
            var newDisplayedOrders = filteredOrders
                .Take(Constants.MaxOrdersNumberOnPageInOrdersList)
                .ToList();
            PrevOrdersPageIsAvailable = _ordersListOffset > 0;
            NextOrdersPageIsAvailable = 
                newDisplayedOrders.Count < Task.Run(() => filteredOrders.Count()).Result;

            OrdersList.Clear();
            foreach (var o in newDisplayedOrders)
            {
                OrdersList.Add(o);
            }
            OnPropertyChanged(nameof(OrdersListIsNotEmpty));
            OnPropertyChanged(nameof(OrdersListIsEmpty));
        }
        
        void UpdateOrderBooksContent(bool clearOffset = true)
        {
            if (clearOffset)
            {
                _orderBooksListOffset = 0;
            }

            using var db = new DbUnit();
            var orderedBooks = db.OrderedBooks.GetAll()
                .Where(ob => ob.IdOrder == _currOrder.Id)
                .Skip(_orderBooksListOffset);
            var newDisplayedOrderBooks = orderedBooks
                .Take(Constants.MaxBooksNumberOnPageInOrder)
                .ToList();
            PrevPageIsAvailable = _orderBooksListOffset > 0;
            NextPageIsAvailable = 
                newDisplayedOrderBooks.Count < orderedBooks.Count();

            OrderBooks.Clear();
            foreach (var ob in newDisplayedOrderBooks)
            {
                OrderBooks.Add(ob);
            }
            OnPropertyChanged(nameof(ContentIsNotEmpty));
            OnPropertyChanged(nameof(ContentIsEmpty));
        }

        void SetOrderToView(Order order)
        {
            using var db = new DbUnit();

            CurrOrder = new(order);
            CurrOrderCost = db.OrderedBooks.GetOrderCostByOrderId(order.Id);

            UpdateOrderBooksContent();
        }
    }
}
