using LibApp.Models;
using LibApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LibApp
{
    public partial class MainWindow : Window
    {
        public enum MainWindowTab
        {
            CatalogueTabAU,
            BookAddTabA,
            BookEditTabA,
            FavouriteTabU,
            CartTabU,
            OrdersHistoryTabAU,
            AboutTabAU,
            AccountTabAU,
            BookInfoTabU,
            OrderingTabU,
        }
        TabItem? EnumTabToTab(MainWindowTab tab)
        {
            return tab switch
            {
                MainWindowTab.CatalogueTabAU => CatalogueTabAU,
                MainWindowTab.BookAddTabA => BookAddTabA,
                MainWindowTab.BookEditTabA => BookEditTabA,
                MainWindowTab.FavouriteTabU => FavouriteTabU,
                MainWindowTab.CartTabU => CartTabU,
                MainWindowTab.OrdersHistoryTabAU => OrdersHistoryTabAU,
                MainWindowTab.AboutTabAU => AboutTabU,
                MainWindowTab.AccountTabAU => AccountTabAU,
                MainWindowTab.BookInfoTabU => BookInfoTabU,
                MainWindowTab.OrderingTabU => OrderingTabU,
                _ => null
            };
        }

        public MainWindow(Login user)
        {
            InitializeComponent();
            DataContext = new MainVM(user);
            if (((MainVM)DataContext).IsAdmin)
            {
                FavouriteTabU.Visibility = Visibility.Collapsed;
                CartTabU.Visibility = Visibility.Collapsed;
            }
            else
            {
                BookAddTabA.Visibility = Visibility.Collapsed;
            }
            // Common
            CatalogueTabAUContent.DataContext = new CatalogueTabVM(user);
            OrdersHistoryTabAUContent.DataContext = new OrdersHistoryTabVM(user);
            AboutTabAUContent.DataContext = new AboutTabVM();
            AccountTabAUContent.DataContext = new AccountTabVM(this, user);
            // Admin
            BookAddTabAContent.DataContext = new BookAddTabVM();
            BookEditTabAContent.DataContext = new BookEditTabVM();
           // User
            FavouriteTabUContent.DataContext = new FavouriteTabVM(user);
            CartTabUContent.DataContext = new CartTabVM(user);
            BookInfoTabUContent.DataContext = new BookInfoTabVM(user);
            OrderingTabUContent.DataContext = new OrderingTabVM(user);

            VMInteractionsManager.TabSelecting += SelectTab;
            VMInteractionsManager.TabCollapsing += CollapseTab;
            VMInteractionsManager.MainWindowClosing += CloseWindow;
            VMInteractionsManager.OnTabSelected(MainWindowTab.CatalogueTabAU);
        }

        void SelectTab(MainWindowTab tab)
        {
            TabItem? tabTI = EnumTabToTab(tab);
            if (tabTI is null)
            {
                return;
            }
            tabTI.Visibility = Visibility.Visible;
            tabTI.IsSelected = true;
        }
        void CollapseTab(MainWindowTab tab)
        {
            TabItem? tabTI = EnumTabToTab(tab);
            if (tabTI is null)
            {
                return;
            }
            tabTI.Visibility = Visibility.Collapsed;
        }
        void CloseWindow()
        {
            VMInteractionsManager.MainWindowClosing -= CloseWindow;
            Close();
        }
    }
}
