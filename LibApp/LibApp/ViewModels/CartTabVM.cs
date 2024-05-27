using LibApp.Models;
using LibApp.Repositories;
using LinqToDB;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class CartTabVM : BaseVM
    {
        readonly Login _user;

        int _cartListOffset = 0;
        bool _nextPageIsAvailable;
        bool _prevPageIsAvailable = false;

        public ObservableCollection<Book> CartBooks { get; set; }

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

        public bool ContentIsNotEmpty => CartBooks?.Any() ?? false;
        public bool ContentIsEmpty => !ContentIsNotEmpty;
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

        public CartTabVM(Login user)
        {
            _user = user;

            CartBooks = new ObservableCollection<Book>();
            VMInteractionsManager.BooksUpdated += () => UpdateCartBooksContent();
            VMInteractionsManager.FavsUpdated += UpdateFavButtons;
            VMInteractionsManager.CartUpdated += () => UpdateCartBooksContent();
            VMInteractionsManager.BookCountInCartUpdated += UpdateCartCost;

            CartBooksOnSelectionChangedCmd = new(ExecuteCartBooksOnSelectionChangedCmd);
            MakeOrderCmd = new(ExecuteMakeOrderCmd);
            RemoveAllFromCartCmd = new(ExecuteRemoveAllFromCartCmd);
            NextPageCmd = new(ExecuteNextPageCmd);
            PrevPageCmd = new(ExecutePrevPageCmd);
            RemoveFromCartCmd = new(ExecuteRemoveFromCartCmd);
            AddToFavouriteCmd = new(ExecuteAddToFavouriteCmd);
            RemoveFromFavouriteCmd = new(ExecuteRemoveFromFavouriteCmd);
            CartBookCountOnValueChangedCmd = new(ExecuteCartBookCountOnValueChangedCmd);

            UpdateCartBooksContent();
        }

        public RelayCommand CartBooksOnSelectionChangedCmd { get; init; }
        public RelayCommand MakeOrderCmd { get; init; }
        public RelayCommand RemoveAllFromCartCmd { get; init; }
        public RelayCommand NextPageCmd { get; init; }
        public RelayCommand PrevPageCmd { get; init; }
        public RelayCommand RemoveFromCartCmd { get; init; }
        public RelayCommand AddToFavouriteCmd { get; init; }
        public RelayCommand RemoveFromFavouriteCmd { get; init; }
        public RelayCommand CartBookCountOnValueChangedCmd { get; init; }

        void ExecuteCartBooksOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ListView lv && lv.SelectedItem is Book b))
            {
                return;
            }

            lv.SelectedItem = null;

            VMInteractionsManager.OnBookToViewSelected(b);
            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookInfoTabU);
        }
        void ExecuteMakeOrderCmd(object? obj)
        {
            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.OrderingTabU);
        }
        void ExecuteRemoveAllFromCartCmd(object? obj)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                Task.Run(() => db.Carts.DeleteByUserIdAsync(_user.Id)).Wait();
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return;
            }
            VMInteractionsManager.OnCartUpdated();
        }
        void ExecuteNextPageCmd(object? obj)
        {
            _cartListOffset += Constants.MaxBooksNumberOnPageInCart;
            UpdateCartBooksContent(false);
        }
        void ExecutePrevPageCmd(object? obj)
        {
            _cartListOffset -= Constants.MaxBooksNumberOnPageInCart;
            UpdateCartBooksContent(false);
        }
        void ExecuteRemoveFromCartCmd(object? obj)
        {
            if (obj is not long bookId)
            {
                ProgramError.Show("obj is not long bookId");
                return;
            }

            if (CommandFunctionsStore.RemoveFromCartCmdFunc(_user.Id, bookId))
            {
                VMInteractionsManager.OnCartUpdated();
            }
        }
        void ExecuteAddToFavouriteCmd(object? obj)
        {
            if (obj is not long bookId)
            {
                ProgramError.Show("obj is not long bookId");
                return;
            }

            if (CommandFunctionsStore.AddToFavouriteCmdFunc(_user.Id, bookId))
            {
                VMInteractionsManager.OnCartUpdated();
            }
        }
        void ExecuteRemoveFromFavouriteCmd(object? obj)
        {
            if (obj is not long bookId)
            {
                ProgramError.Show("obj is not long bookId");
                return;
            }

            if (CommandFunctionsStore.RemoveFromFavouriteCmdFunc(_user.Id, bookId))
            {
                VMInteractionsManager.OnFavsUpdated();
            }
        }
        void ExecuteCartBookCountOnValueChangedCmd(object? obj)
        {
            if (!(obj is Xceed.Wpf.Toolkit.IntegerUpDown iup && iup.Tag is long bookId))
            {
                ProgramError.Show("!(obj is Xceed.Wpf.Toolkit.IntegerUpDown iup && iup.Tag is long bookId)");
                return;
            }

            if (iup.Value is null || iup.Value <= 0)
            {
                ExecuteRemoveFromCartCmd(bookId);
                return;
            }

            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                var b = Task.Run(() => db.Carts.GetAll().FirstOrDefaultAsync(c => c.IdClient == _user.Id && c.IdBook == bookId)).Result;
                if (b is not null)
                { 
                    b.Count = (int)iup.Value;
                }
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return;
            }
            VMInteractionsManager.OnBookCountInCartUpdated();
        }

        void UpdateCartBooksContent(bool clearOffset = true)
        {
            if (clearOffset)
            {
                _cartListOffset = 0;
            }

            using var db = new DbUnit();
            var cartBooks = db.Carts.GetBooksByUserId(_user.Id);
            cartBooks = cartBooks.Skip(_cartListOffset);
            var newDisplayedBooks = cartBooks
                .Take(Constants.MaxBooksNumberOnPageInCart).ToList();
            PrevPageIsAvailable = _cartListOffset > 0;
            NextPageIsAvailable = newDisplayedBooks.Count < cartBooks.Count();

            CartBooks.Clear();
            foreach (var b in newDisplayedBooks)
            {
                CartBooks.Add(b);
            }

            UpdateCartCost();
            OnPropertyChanged(nameof(ContentIsNotEmpty));
            OnPropertyChanged(nameof(ContentIsEmpty));
        }
        void UpdateFavButtons()
        {
            OnPropertyChanged(nameof(UserId));
        }
        void UpdateCartCost()
        {
            OnPropertyChanged(nameof(Cost));
        }
    }
}
