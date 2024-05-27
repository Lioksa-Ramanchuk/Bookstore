using LibApp.Models;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using LibApp.Repositories;
using System.Linq;
using LibApp.ViewModels.SubViewModels;
using LibApp.Converters;

namespace LibApp.ViewModels
{
    public class BookEditTabVM : BaseVM
    {
        BookVM _bookToEdit = new(new()
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

        public BookVM BookToEdit
        {
            get => _bookToEdit;
            set { _bookToEdit = value; OnPropertyChanged(nameof(BookToEdit)); }
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

        public BookEditTabVM()
        {
            VMInteractionsManager.BookToEditSelecting += SetBookToEdit;

            SelectImgCmd = new(ExecuteSelectImgCmd);
            EditBookCmd = new(ExecuteEditBookCmd);
            CancelCmd = new(ExecuteCancelCmd);
        }

        public RelayCommand SelectImgCmd { get; init; }
        public RelayCommand EditBookCmd { get; init; }
        public RelayCommand CancelCmd { get; init; }

        void ExecuteSelectImgCmd(object? obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files | *.bmp; *.jpg; *.jpeg; *.png; ..."
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _bookToEdit.Image = openFileDialog.FileName;
            }
        }
        void ExecuteEditBookCmd(object? obj)
        {
            if (string.IsNullOrWhiteSpace(_bookToEdit.Title))
            {
                ErrorMsg = "Увядзіце назву кнігі";
            }
            else if (string.IsNullOrWhiteSpace(_bookToEdit.Author))
            {
                ErrorMsg = "Увядзіце імя аўтар_кі";
            }
            else
            {
                using var db = new DbUnit();

                Book b = new()
                {
                    Id = _bookToEdit.Id,
                    Title = _bookToEdit.Title,
                    Author = _bookToEdit.Author,
                    Year = _bookToEdit.Year,
                    Pages = _bookToEdit.Pages,
                    Description = _bookToEdit.Description,
                    Tags = ConvertersStore.InputStrToStrArrayConverter.ConvertBack(_bookToEdit.Tags.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), null, null, null) as string
                                                                       ?? string.Empty,
                    Price = _bookToEdit.Price,
                    Count = _bookToEdit.Count,
                    Publisher = _bookToEdit.Publisher,
                    Isbn = _bookToEdit.Isbn,
                    Image = _bookToEdit.Image,
                };

                using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
                try
                {
                    db.Books.Update(b);
                    Task.Run(db.SaveAsync).Wait();
                    Task.Run(() => tran.CommitAsync()).Wait();
                }
                catch (Exception ex)
                {
                    Task.Run(() => tran.RollbackAsync()).Wait();
                    ProgramError.Show($"{ex}");
                    return;
                }

                ErrorMsg = string.Empty;
                VMInteractionsManager.OnBooksUpdated();
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.CatalogueTabAU);
                VMInteractionsManager.OnTabCollapsed(MainWindow.MainWindowTab.BookEditTabA);
            }
        }

        void ExecuteCancelCmd(object? obj)
        {
            ErrorMsg = string.Empty;
            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.CatalogueTabAU);
            VMInteractionsManager.OnTabCollapsed(MainWindow.MainWindowTab.BookEditTabA);
        }

        void SetBookToEdit(Book book)
        {
            BookToEdit = new(book);
        }
    }
}
