using LibApp.Models;
using System;
using static LibApp.MainWindow;

namespace LibApp
{
    public static class VMInteractionsManager
    {
        public static event Action<Book>? BookToEditSelecting;
        public static event Action<Book>? BookToViewSelecting;
        public static event Action<MainWindowTab>? TabSelecting;
        public static event Action<MainWindowTab>? TabCollapsing;
        public static event Action? AuthWindowClosing;
        public static event Action? MainWindowClosing;
        public static event Action? BooksUpdated;
        public static event Action? FavsUpdated;
        public static event Action? CartUpdated;
        public static event Action? BookCountInCartUpdated;
        public static event Action? OrdersUpdated;

        public static void OnBookToEditSelected(Book book) => BookToEditSelecting?.Invoke(book);
        public static void OnBookToViewSelected(Book book) => BookToViewSelecting?.Invoke(book);
        public static void OnTabSelected(MainWindowTab tab) => TabSelecting?.Invoke(tab);
        public static void OnTabCollapsed(MainWindowTab tab) => TabCollapsing?.Invoke(tab);
        public static void OnAuthWindowClosing() => AuthWindowClosing?.Invoke();
        public static void OnMainWindowClosing() => MainWindowClosing?.Invoke();
        public static void OnBooksUpdated() => BooksUpdated?.Invoke();
        public static void OnFavsUpdated() => FavsUpdated?.Invoke();
        public static void OnCartUpdated() => CartUpdated?.Invoke();
        public static void OnBookCountInCartUpdated() => BookCountInCartUpdated?.Invoke();
        public static void OnOrdersUpdated() => OrdersUpdated?.Invoke();
    }
}