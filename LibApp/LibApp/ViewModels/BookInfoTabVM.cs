using LibApp.Models;
using LibApp.Repositories;
using LibApp.ViewModels.SubViewModels;
using System;
using System.Threading.Tasks;

namespace LibApp.ViewModels
{
    public class BookInfoTabVM : BaseVM
    {
        readonly Login _user;
        BookVM _bookToView = new(new()
        {
            Id = 0,
            Title = string.Empty,
            Author = string.Empty,
            Year = (short)DateTime.Now.Year,
            Pages = 0,
            Description = string.Empty,
            Tags = string.Empty,
            Price = 0,
            Count = 0,
            Publisher = string.Empty,
            Isbn = string.Empty,
            Image = Constants.BookDefaultImagePath,
        });

        public BookVM BookToView
        {
            get => _bookToView;
            set { _bookToView = value; OnPropertyChanged(nameof(BookToView)); }
        }
        public long UserId { get => _user.Id; set => _ = value; }

        public BookInfoTabVM(Login user)
        {
            _user = user;
            VMInteractionsManager.BookToViewSelecting += SetBookToView;
            VMInteractionsManager.BooksUpdated += UpdateInfo;
            VMInteractionsManager.FavsUpdated += UpdateFavCartButtons;
            VMInteractionsManager.CartUpdated += UpdateFavCartButtons;

            AddToFavouriteCmd = new(ExecuteAddToFavouriteCmd);
            RemoveFromFavouriteCmd = new(ExecuteRemoveFromFavouriteCmd);
            AddToCartCmd = new(ExecuteAddToCartCmd);
            RemoveFromCartCmd = new(ExecuteRemoveFromCartCmd);
        }

        public RelayCommand AddToFavouriteCmd { get; init; }
        public RelayCommand RemoveFromFavouriteCmd { get; init; }
        public RelayCommand AddToCartCmd { get; init; }
        public RelayCommand RemoveFromCartCmd { get; init; }

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

        void SetBookToView(Book book)
        {
            BookToView = new(book);
        }
        void UpdateInfo()
        {
            using var db = new DbUnit();
            if (BookToView.Id == 0 || Task.Run(() => db.Books.GetAsync(BookToView.Id)).Result is not Book b)
            {
                return;
            }
            SetBookToView(b);
        }
        void UpdateFavCartButtons()
        {
            OnPropertyChanged(nameof(UserId));
        }
    }
}
