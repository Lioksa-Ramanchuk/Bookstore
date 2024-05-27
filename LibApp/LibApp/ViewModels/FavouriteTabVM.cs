using LibApp.Converters;
using LibApp.Models;
using LibApp.Repositories;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class FavouriteTabVM : BaseVM
    {
        readonly Login _user;

        string _textToSearch = string.Empty;
        int _favListOffset = 0;
        bool _nextPageIsAvailable;
        bool _prevPageIsAvailable = false;

        ExpressionStarter<Book> _totalFilter = null!;
        ExpressionStarter<Book> _textFilter;

        public ObservableCollection<Book> FavBooks { get; set; }
        public string TextToSearch
        {
            get => _textToSearch;
            set
            {
                _textToSearch = value;
                OnPropertyChanged(nameof(TextToSearch));

                string[] strsToSearch = ConvertersStore.InputStrToStrArrayConverter.Convert(_textToSearch, null, null, null) as string[]
                                        ?? Array.Empty<string>();

                if (!strsToSearch.Any())
                {
                    _textFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _textFilter = PredicateBuilder.New<Book>();
                    foreach (var str in strsToSearch)
                    {
                        _textFilter = _textFilter
                            .Or(b => EF.Functions.Like(b.Title, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Author, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Publisher, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Tags, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Tags, $"%;{str}%"))
                            .Or(b => b.Description.Contains(str))
                            .Or(b => EF.Functions.Like(b.Isbn, $"{str}%"));

                        foreach (var word in str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            _textFilter = _textFilter
                                .Or(b => EF.Functions.Like(b.Author, $"{word}%"))
                                .Or(b => EF.Functions.Like(b.Author, $"%[ -]{word}%"));
                        }

                        if (short.TryParse(str, out short wordAsShort))
                        {
                            _textFilter = _textFilter
                                .Or(b => b.Year == wordAsShort);
                        }
                    }
                }
                UpdateFilterAndFavList();
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
        public bool ContentIsNotEmpty => FavBooks?.Any() ?? false;
        public bool ContentIsEmpty => !ContentIsNotEmpty;
        public long UserId { get => _user.Id; set => _ = value; }

        public FavouriteTabVM(Login user)
        {
            _user = user;

            FavBooks = new ObservableCollection<Book>();
            VMInteractionsManager.BooksUpdated += () => UpdateFavBooksContent();
            VMInteractionsManager.FavsUpdated += () => UpdateFavBooksContent();
            VMInteractionsManager.CartUpdated += UpdateCartButtons;

            FavBooksOnSelectionChangedCmd = new(ExecuteFavBooksOnSelectionChangedCmd);
            AddAllFavsToCartCmd = new(ExecuteAddAllFavsToCartCmd);
            RemoveAllFavsCmd = new(ExecuteRemoveAllFavsCmd);
            NextPageCmd = new(ExecuteNextPageCmd);
            PrevPageCmd = new(ExecutePrevPageCmd);
            RemoveFromFavouriteCmd = new(ExecuteRemoveFromFavouriteCmd);
            AddToCartCmd = new(ExecuteAddToCartCmd);
            RemoveFromCartCmd = new(ExecuteRemoveFromCartCmd);

            _textFilter = PredicateBuilder.New<Book>(true);

            UpdateFilterAndFavList();
        }

        public RelayCommand FavBooksOnSelectionChangedCmd { get; init; }
        public RelayCommand AddAllFavsToCartCmd { get; init; }
        public RelayCommand RemoveAllFavsCmd { get; init; }
        public RelayCommand NextPageCmd { get; init; }
        public RelayCommand PrevPageCmd { get; init; }
        public RelayCommand RemoveFromFavouriteCmd { get; init; }
        public RelayCommand AddToCartCmd { get; init; }
        public RelayCommand RemoveFromCartCmd { get; init; }

        void ExecuteFavBooksOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ListView lv && lv.SelectedItem is Book b))
            {
                return;
            }

            lv.SelectedItem = null;

            VMInteractionsManager.OnBookToViewSelected(b);
            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookInfoTabU);
        }
        void ExecuteAddAllFavsToCartCmd(object? obj)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                db.Favourites.AddAllToCartByUserId(_user.Id);
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
        void ExecuteRemoveAllFavsCmd(object? obj)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                Task.Run(() => db.Favourites.DeleteByUserIdAsync(_user.Id)).Wait();
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return;
            }
            VMInteractionsManager.OnFavsUpdated();
        }
        void ExecuteNextPageCmd(object? obj)
        {
            _favListOffset += Constants.MaxBooksNumberOnPageInFavourite;
            UpdateFavBooksContent(false);
        }
        void ExecutePrevPageCmd(object? obj)
        {
            _favListOffset -= Constants.MaxBooksNumberOnPageInFavourite;
            UpdateFavBooksContent(false);
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
        void ExecuteAddToCartCmd(object? obj)
        {
            if (obj is not long bookId)
            {
                ProgramError.Show("obj is not long bookId");
                return;
            }

            if (CommandFunctionsStore.AddToCartCmdFunc(_user.Id, bookId))
            {
                VMInteractionsManager.OnCartUpdated();
            }
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

        void UpdateFilterAndFavList()
        {
            _totalFilter = PredicateBuilder.New<Book>(true)
                .And(_textFilter);

            UpdateFavBooksContent();
        }
        void UpdateFavBooksContent(bool clearOffset = true)
        {
            if (clearOffset)
            {
                _favListOffset = 0;
            }

            using var db = new DbUnit();
            var filteredBooks = db.Favourites.GetByUserId(_user.Id)
                .Where(_totalFilter);
            filteredBooks = filteredBooks.Skip(_favListOffset);
            var newDisplayedBooks = filteredBooks
                .Take(Constants.MaxBooksNumberOnPageInFavourite).ToList();
            PrevPageIsAvailable = _favListOffset > 0;
            NextPageIsAvailable = newDisplayedBooks.Count < filteredBooks.Count();

            FavBooks.Clear();
            foreach (var b in newDisplayedBooks)
            {
                FavBooks.Add(b);
            }
            OnPropertyChanged(nameof(ContentIsNotEmpty));
            OnPropertyChanged(nameof(ContentIsEmpty));
        }
        void UpdateCartButtons()
        {
            OnPropertyChanged(nameof(UserId));
        }
    }
}