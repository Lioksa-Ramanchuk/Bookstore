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
using System.Windows;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class CatalogueTabVM : BaseVM
    {
        public enum SortMode
        {
            NewFirst,
            CheapFirst,
            ExpensiveFirst,
        }

        readonly Login _user;

        string _textToSearch = string.Empty;
        string _titleSearch = string.Empty;
        string _authorSearch = string.Empty;
        string _publisherSearch = string.Empty;
        decimal _minPriceSearch = 0;
        decimal _maxPriceSearch = Constants.MaxBookPrice;
        string _tagsSearch = string.Empty;
        short _minYearSearch = 0;
        short _maxYearSearch = Constants.MaxBookYear;
        bool _inStockOnlySearch = false;
        SortMode _currentSortMode = SortMode.NewFirst;
        int _catalogueOffset = 0;
        bool _nextPageIsAvailable;
        bool _prevPageIsAvailable = false;

        ExpressionStarter<Book> _totalFilter = null!;

        ExpressionStarter<Book> _textFilter;
        ExpressionStarter<Book> _titleFilter;
        ExpressionStarter<Book> _authorFilter;
        ExpressionStarter<Book> _publisherFilter;
        ExpressionStarter<Book> _minPriceFilter;
        ExpressionStarter<Book> _maxPriceFilter;
        ExpressionStarter<Book> _tagsFilter;
        ExpressionStarter<Book> _minYearFilter;
        ExpressionStarter<Book> _maxYearFilter;
        ExpressionStarter<Book> _inStockOnlyFilter;

        public record SortOption(SortMode Mode, string ModeName);
        public static SortOption[] SortOptions { get; } = new SortOption[]
        {
            new(SortMode.NewFirst, "новыя") ,
            new(SortMode.CheapFirst, "танныя") ,
            new(SortMode.ExpensiveFirst, "дарагія") ,
        };

        public ObservableCollection<Book> Books { get; set; }
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
                UpdateFilterAndCatalogue();
            }
        }
        public string TitleSearch
        {
            get => _titleSearch;
            set
            {
                _titleSearch = value;
                OnPropertyChanged(nameof(TitleSearch));

                string[] strsToSearch = ConvertersStore.InputStrToStrArrayConverter.Convert(_titleSearch, null, null, null) as string[]
                                        ?? Array.Empty<string>();

                if (!strsToSearch.Any())
                {
                    _titleFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _titleFilter = PredicateBuilder.New<Book>();
                    foreach (var str in strsToSearch)
                    {
                        _titleFilter = _titleFilter
                            .Or(b => EF.Functions.Like(b.Title, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Isbn, $"{str}%"));
                    }
                }
                UpdateFilterAndCatalogue();
            }
        }
        public string AuthorSearch
        {
            get => _authorSearch;
            set
            {
                _authorSearch = value;
                OnPropertyChanged(nameof(AuthorSearch));

                string[] strsToSearch = ConvertersStore.InputStrToStrArrayConverter.Convert(_authorSearch, null, null, null) as string[]
                                        ?? Array.Empty<string>();

                if (!strsToSearch.Any())
                {
                    _authorFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _authorFilter = PredicateBuilder.New<Book>();
                    foreach (var str in strsToSearch)
                    {
                        foreach (var word in str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            _authorFilter = _authorFilter
                                .Or(b => EF.Functions.Like(b.Author, $"{word}%"))
                                .Or(b => EF.Functions.Like(b.Author, $"%[ -]{word}%"));
                        }
                    }
                }
                UpdateFilterAndCatalogue();
            }
        }
        public string PublisherSearch
        {
            get => _publisherSearch;
            set
            {
                _publisherSearch = value;
                OnPropertyChanged(nameof(PublisherSearch));

                string[] strsToSearch = ConvertersStore.InputStrToStrArrayConverter.Convert(_publisherSearch, null, null, null) as string[]
                                        ?? Array.Empty<string>();

                if (!strsToSearch.Any())
                {
                    _publisherFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _publisherFilter = PredicateBuilder.New<Book>();
                    foreach (var str in strsToSearch)
                    {
                        _publisherFilter = _publisherFilter
                            .Or(b => EF.Functions.Like(b.Publisher, $"{str}%"));
                    }
                }
                UpdateFilterAndCatalogue();
            }
        }
        public decimal MinPriceSearch
        {
            get => _minPriceSearch;
            set
            {
                _minPriceSearch = value;
                OnPropertyChanged(nameof(MinPriceSearch));

                _minPriceFilter = PredicateBuilder.New<Book>()
                    .Or(b => _minPriceSearch <= b.Price);
                UpdateFilterAndCatalogue();
            }
        }
        public decimal MaxPriceSearch
        {
            get => _maxPriceSearch;
            set
            {
                _maxPriceSearch = value;
                OnPropertyChanged(nameof(MaxPriceSearch));

                _maxPriceFilter = PredicateBuilder.New<Book>()
                    .Or(b => b.Price <= _maxPriceSearch);
                UpdateFilterAndCatalogue();
            }
        }
        public string TagsSearch
        {
            get => _tagsSearch;
            set
            {
                _tagsSearch = value;
                OnPropertyChanged(nameof(TagsSearch));

                string[] strsToSearch = ConvertersStore.InputStrToStrArrayConverter.Convert(_tagsSearch, null, null, null) as string[]
                                        ?? Array.Empty<string>();

                if (!strsToSearch.Any())
                {
                    _tagsFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _tagsFilter = PredicateBuilder.New<Book>();
                    foreach (var str in strsToSearch)
                    {
                        _tagsFilter = _tagsFilter
                            .Or(b => EF.Functions.Like(b.Tags, $"{str}%"))
                            .Or(b => EF.Functions.Like(b.Tags, $"%;{str}%"));
                    }
                }
                UpdateFilterAndCatalogue();
            }
        }
        public short MinYearSearch
        {
            get => _minYearSearch;
            set
            {
                _minYearSearch = value;
                OnPropertyChanged(nameof(MinYearSearch));

                _minYearFilter = PredicateBuilder.New<Book>()
                    .Or(b => _minYearSearch <= b.Year);
                UpdateFilterAndCatalogue();
            }

        }
        public short MaxYearSearch
        {
            get => _maxYearSearch;
            set
            {
                _maxYearSearch = value;
                OnPropertyChanged(nameof(MaxYearSearch));

                _maxYearFilter = PredicateBuilder.New<Book>()
                    .Or(b => b.Year <= _maxYearSearch);
                UpdateFilterAndCatalogue();
            }
        }
        public bool InStockOnlySearch
        {
            get => _inStockOnlySearch;
            set
            {
                _inStockOnlySearch = value;
                OnPropertyChanged(nameof(InStockOnlySearch));

                if (!_inStockOnlySearch)
                {
                    _inStockOnlyFilter = PredicateBuilder.New<Book>(true);
                }
                else
                {
                    _inStockOnlyFilter = PredicateBuilder.New<Book>()
                        .Or(b => b.Count > 0);
                }
                UpdateFilterAndCatalogue();
            }
        }
        public SortMode CurrentSortMode
        {
            get => _currentSortMode;
            set { _currentSortMode = value; OnPropertyChanged(nameof(CurrentSortMode)); UpdateCatalogueContent(); }
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

        public bool IsAdmin { get; set; }
        public bool IsNotAdmin => !IsAdmin;
        public bool ContentIsNotEmpty => Books?.Any() ?? false;
        public bool ContentIsEmpty => !ContentIsNotEmpty;
        public long UserId { get => _user.Id; set => _ = value; }

        public CatalogueTabVM(Login user)
        {
            _user = user;
            IsAdmin = user.IdRoleNavigation.Name == Constants.AdministratorRoleName;

            Books = new ObservableCollection<Book>();
            VMInteractionsManager.BooksUpdated += () => UpdateCatalogueContent();
            VMInteractionsManager.FavsUpdated += UpdateFavCartButtons;
            VMInteractionsManager.CartUpdated += UpdateFavCartButtons;

            CatalogueSortOnSelectionChangedCmd = new(ExecuteCatalogueSortOnSelectionChangedCmd);
            CatalogueOnSelectionChangedCmd = new(ExecuteCatalogueOnSelectionChangedCmd);
            NextPageCmd = new(ExecuteNextPageCmd);
            PrevPageCmd = new(ExecutePrevPageCmd);
            EditBookCmd = new(ExecuteEditBookCmd);
            AddToFavouriteCmd = new(ExecuteAddToFavouriteCmd);
            RemoveFromFavouriteCmd = new(ExecuteRemoveFromFavouriteCmd);
            AddToCartCmd = new(ExecuteAddToCartCmd);
            RemoveFromCartCmd = new(ExecuteRemoveFromCartCmd);

            _textFilter = PredicateBuilder.New<Book>(true);
            _titleFilter = PredicateBuilder.New<Book>(true);
            _authorFilter = PredicateBuilder.New<Book>(true);
            _publisherFilter = PredicateBuilder.New<Book>(true);
            _minPriceFilter = PredicateBuilder.New<Book>()
                .Or(b => MinPriceSearch <= b.Price);
            _maxPriceFilter = PredicateBuilder.New<Book>()
                .Or(b => b.Price <= MaxPriceSearch);
            _tagsFilter = PredicateBuilder.New<Book>(true);
            _minYearFilter = PredicateBuilder.New<Book>()
                .Or(b => MinYearSearch <= b.Year);
            _maxYearFilter = PredicateBuilder.New<Book>()
                .Or(b => b.Year <= MaxYearSearch);
            _inStockOnlyFilter = PredicateBuilder.New<Book>(true);

            UpdateFilterAndCatalogue();
        }

        public RelayCommand CatalogueSortOnSelectionChangedCmd { get; init; }
        public RelayCommand CatalogueOnSelectionChangedCmd { get; init; }
        public RelayCommand NextPageCmd { get; init; }
        public RelayCommand PrevPageCmd { get; init; }
        public RelayCommand EditBookCmd { get; init; }
        public RelayCommand AddToFavouriteCmd { get; init; }
        public RelayCommand RemoveFromFavouriteCmd { get; init; }
        public RelayCommand AddToCartCmd { get; init; }
        public RelayCommand RemoveFromCartCmd { get; init; }

        void ExecuteCatalogueSortOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ComboBox cb && cb.SelectedItem is SortOption option))
            {
                return;
            }

            CurrentSortMode = option.Mode;
        }
        void ExecuteCatalogueOnSelectionChangedCmd(object? obj)
        {
            if (!(obj is ListView lv && lv.SelectedItem is Book b))
            {
                return;
            }

            lv.SelectedItem = null;

            if (IsAdmin)
            {
                VMInteractionsManager.OnBookToEditSelected(b);
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookEditTabA);
            }
            else
            {
                VMInteractionsManager.OnBookToViewSelected(b);
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookInfoTabU);
            }
        }
        void ExecuteNextPageCmd(object? obj)
        {
            _catalogueOffset += Constants.MaxBooksNumberOnPageInCatalogue;
            UpdateCatalogueContent(false);
        }
        void ExecutePrevPageCmd(object? obj)
        {
            _catalogueOffset -= Constants.MaxBooksNumberOnPageInCatalogue;
            UpdateCatalogueContent(false);
        }
        void ExecuteEditBookCmd(object? obj)
        {
            if (obj is not long id)
            {
                return;
            }

            using var db = new DbUnit();
            var b = Task.Run(() => db.Books.Get(id)).Result;
            if (b is null)
            {
                ProgramError.Show("Кніга не знойдзеная");
                return;
            }
            VMInteractionsManager.OnBookToEditSelected(b);
            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.BookEditTabA);
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
                VMInteractionsManager.OnFavsUpdated();
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

        void UpdateFilterAndCatalogue()
        {
            _totalFilter = PredicateBuilder.New<Book>(true)
                .And(_textFilter)
                .And(_titleFilter)
                .And(_authorFilter)
                .And(_publisherFilter)
                .And(_minPriceFilter)
                .And(_maxPriceFilter)
                .And(_tagsFilter)
                .And(_minYearFilter)
                .And(_maxYearFilter)
                .And(_inStockOnlyFilter);

            UpdateCatalogueContent();
        }
        void UpdateCatalogueContent(bool clearOffset = true)
        {
            if (clearOffset)
            {
                _catalogueOffset = 0;
            }

            using var db = new DbUnit();
            var filteredBooks = db.Books.GetAll()
                .Where(_totalFilter);
            filteredBooks = _currentSortMode switch
            {
                SortMode.NewFirst => filteredBooks.OrderByDescending(b => b.Year),
                SortMode.CheapFirst => filteredBooks.OrderBy(b => b.Price),
                SortMode.ExpensiveFirst => filteredBooks.OrderByDescending(b => b.Price),
                _ => filteredBooks
            };
            filteredBooks = filteredBooks.Skip(_catalogueOffset);
            var newDisplayedBooks = filteredBooks
                .Take(Constants.MaxBooksNumberOnPageInCatalogue)
                .ToList();
            PrevPageIsAvailable = _catalogueOffset > 0;
            NextPageIsAvailable = 
                newDisplayedBooks.Count < Task.Run(() => filteredBooks.Count()).Result;

            Books.Clear();
            foreach (var b in newDisplayedBooks)
            {
                Books.Add(b);
            }
            OnPropertyChanged(nameof(ContentIsNotEmpty));
            OnPropertyChanged(nameof(ContentIsEmpty));
        }
        void UpdateFavCartButtons()
        {
            OnPropertyChanged(nameof(UserId));
        }
    }
}
