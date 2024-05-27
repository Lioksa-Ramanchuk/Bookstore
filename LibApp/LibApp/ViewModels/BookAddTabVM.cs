using LibApp.Converters;
using LibApp.Models;
using LibApp.Repositories;
using LibApp.ViewModels.SubViewModels;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibApp.ViewModels
{
    public class BookAddTabVM : BaseVM
    {
        BookVM _bookToAdd = new(new()
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

        public BookVM BookToAdd
        {
            get => _bookToAdd;
            set { _bookToAdd = value; OnPropertyChanged(nameof(BookToAdd)); }
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

        public BookAddTabVM()
        {            
            AddBookCmd = new(ExecuteAddBookCmd);
            SelectImgCmd = new(ExecuteSelectImgCmd);
        }

        public RelayCommand SelectImgCmd { get; init; }
        public RelayCommand AddBookCmd { get; init; }

        void ExecuteAddBookCmd(object? obj)
        {
            if (string.IsNullOrWhiteSpace(_bookToAdd.Title))
            {
                ErrorMsg = "Увядзіце назву кнігі";
            }
            else if (string.IsNullOrWhiteSpace(_bookToAdd.Author))
            {
                ErrorMsg = "Увядзіце імя аўтар_кі";
            }
            else
            {
                Book b = new()
                {
                    Title = _bookToAdd.Title.Trim(),
                    Author = _bookToAdd.Author.Trim(),
                    Year = _bookToAdd.Year,
                    Pages = _bookToAdd.Pages,
                    Description = _bookToAdd.Description.Trim(),
                    Tags = ConvertersStore.InputStrToStrArrayConverter.ConvertBack(_bookToAdd.Tags.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), null, null, null) as string 
                                                                       ?? string.Empty,
                    Price = _bookToAdd.Price,
                    Count = _bookToAdd.Count,
                    Publisher = _bookToAdd.Publisher.Trim(),
                    Isbn = _bookToAdd.Isbn.Trim(),
                    Image = _bookToAdd.Image.Trim(),
                };

                using var db = new DbUnit();
                using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
                try
                {
                    db.Books.Create(b);
                    Task.Run(db.SaveAsync).Wait();
                    Task.Run(() => tran.CommitAsync()).Wait();
                }
                catch (Exception ex)
                {
                    Task.Run(() => tran.RollbackAsync()).Wait();
                    ProgramError.Show($"{ex}");
                    return;
                }
                VMInteractionsManager.OnBooksUpdated();
                VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.CatalogueTabAU);
                ClearContent();
            }
        }
        void ExecuteSelectImgCmd(object? obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files | *.bmp; *.jpg; *.jpeg; *.png; ..."
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _bookToAdd.Image = openFileDialog.FileName;
            }
        }

        void ClearContent()
        {
            BookToAdd = new(new()
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
            ErrorMsg = string.Empty;
        }
    }
}
