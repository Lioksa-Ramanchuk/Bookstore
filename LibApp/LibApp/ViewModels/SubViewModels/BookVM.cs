using LibApp.Models;

namespace LibApp.ViewModels.SubViewModels
{
    public class BookVM : BaseVM
    {
        readonly Book _book;

        public long Id
        {
            get => _book.Id;
            set { _book.Id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string Title
        {
            get => _book.Title;
            set { _book.Title = value; OnPropertyChanged(nameof(Title)); }
        }
        public string Author
        {
            get => _book.Author;
            set { _book.Author = value; OnPropertyChanged(nameof(Author)); }
        }
        public short Year
        {
            get => _book.Year;
            set { _book.Year = value; OnPropertyChanged(nameof(Year)); }
        }
        public short Pages
        {
            get => _book.Pages;
            set { _book.Pages = value; OnPropertyChanged(nameof(Pages)); }
        }
        public string Description
        {
            get => _book.Description;
            set { _book.Description = value; OnPropertyChanged(nameof(Description)); }
        }
        public string Tags
        {
            get => _book.Tags;
            set { _book.Tags = value; OnPropertyChanged(nameof(Tags)); }
        }
        public decimal Price
        {
            get => _book.Price;
            set { _book.Price = value; OnPropertyChanged(nameof(Price)); }
        }
        public int Count
        {
            get => _book.Count;
            set { _book.Count = value; OnPropertyChanged(nameof(Count)); }
        }
        public string Publisher
        {
            get => _book.Publisher;
            set { _book.Publisher = value; OnPropertyChanged(nameof(Publisher)); }
        }
        public string Isbn
        {
            get => _book.Isbn;
            set { _book.Isbn = value; OnPropertyChanged(nameof(Isbn)); }
        }
        public string Image
        {
            get => _book.Image;
            set { _book.Image = value; OnPropertyChanged(nameof(Image)); }
        }

        public BookVM(Book b)
        {
            _book = b;
        }
    }
}
