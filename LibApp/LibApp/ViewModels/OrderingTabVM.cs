using LibApp.Models;
using LibApp.Repositories;
using LibApp.ViewModels.SubViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibApp.ViewModels
{
    public class OrderingTabVM : BaseVM
    {
        readonly Login _user;
        OrderVM _orderToMake = new(new()
        {
            Name = string.Empty,
            Email = string.Empty,
            Address = string.Empty,
            Comment = string.Empty,
        });

        public OrderVM OrderToMake
        {
            get => _orderToMake;
            set { _orderToMake = value; OnPropertyChanged(nameof(OrderToMake)); }
        }

        string _errorMsg = string.Empty;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                _errorMsg = value;
                OnPropertyChanged(nameof(ErrorMsg));
            }
        }

        public long UserId { get => _user.Id; set => _ = value; }
        public decimal Cost
        {
            get
            {
                using var db = new DbUnit();
                decimal cost = 0;
                try
                {
                    cost = db.Carts.GetCostByUserId(_user.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex}", "Падлік кошту заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return cost;
            }
        }

        public OrderingTabVM(Login user)
        {
            _user = user;

            VMInteractionsManager.BookCountInCartUpdated += UpdateOrderCost;

            MakeOrderCmd = new(ExecuteMakeOrderCmd);
        }

        public RelayCommand MakeOrderCmd { get; init; }

        void ExecuteMakeOrderCmd(object? obj)
        {
            using var db = new DbUnit();
            if (!db.Carts.GetBooksByUserId(_user.Id).Any())
            {
                ErrorMsg = "Ваш кошык пусты";
            }
            else if (string.IsNullOrWhiteSpace(_orderToMake.Name))
            {
                ErrorMsg = "Увядзіце імя";
            }
            else if (!Constants.EmailRgx.IsMatch(_orderToMake.Email))
            {
                ErrorMsg = "Некарэктны Email";
            }
            else if (string.IsNullOrWhiteSpace(_orderToMake.Address))
            {
                ErrorMsg = "Увядзіце адрас";
            }
            else
            {
                var notEnoughBooks = db.Carts.GetWhereNotEnough(_user.Id).Select(c => c.IdBookNavigation);

                int nNotEnoughBooks = notEnoughBooks.Count();
                if (nNotEnoughBooks != 0)
                {
                    var confirmOrder = WPFCustomMessageBox.CustomMessageBox.ShowYesNo($"""
                        У вашым кошыку ёсць кнігі, якіх недастаткова на складзе:
                        {string.Join(", ", notEnoughBooks
                               .Take(Constants.MaxNotEnoughBooksDisplayedNumber)
                               .Select(b => b.Title).AsEnumerable())}{(nNotEnoughBooks > Constants.MaxNotEnoughBooksDisplayedNumber ? "..." : string.Empty)}

                        Усё роўна пацвердзіць заказ?
                        """, "Пацвярджэнне заказа", "Пацвердзіць", "Скасаваць", MessageBoxImage.Warning);

                    if (confirmOrder == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
                try
                {
                    var order = new Order()
                    {
                        IdClient = _user.Id,
                        Name = _orderToMake.Name,
                        Email = _orderToMake.Email,
                        Address = _orderToMake.Address,
                        Comment = _orderToMake.Comment,
                        Date = DateTime.Now,
                        IdStatus = db.OrderStatuses.GetByName(Constants.ProcessingOrderStatusName)?.Id ?? 1,
                    };
                    db.Orders.Create(order);
                    Task.Run(db.SaveAsync).Wait();
                    Task.Run(() => db.OrderedBooks.OrderCartAsync(_user.Id, order.Id)).Wait();
                    Task.Run(() => db.Carts.DeleteByUserIdAsync(_user.Id)).Wait();
                    Task.Run(db.SaveAsync).Wait();
                    Task.Run(() => tran.CommitAsync()).Wait();
                    MessageBox.Show($"Ваш заказ паспяхова аформлены!\nНумар заказа: {order.Id}", "Афармленне заказа",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ProgramError.Show($"{ex}");
                    return;
                }
                VMInteractionsManager.OnCartUpdated();
                VMInteractionsManager.OnOrdersUpdated();
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.CatalogueTabAU);
                VMInteractionsManager.OnTabCollapsed(MainWindow.MainWindowTab.OrderingTabU);
                Clear();
            }
        }

        void UpdateOrderCost()
        {
            OnPropertyChanged(nameof(Cost));
        }

        void Clear()
        {
            OrderToMake = new(new()
            {
                Name = string.Empty,
                Email = string.Empty,
                Address = string.Empty,
                Comment = string.Empty,
            });
            ErrorMsg = string.Empty;
        }
    }
}
