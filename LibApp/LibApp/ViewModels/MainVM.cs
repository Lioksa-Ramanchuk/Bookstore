using LibApp.Models;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class MainVM : BaseVM
    {
        public bool IsAdmin { get; set; }
        public bool IsNotAdmin => !IsAdmin;

        public MainVM(Login user)
        {
            IsAdmin = (user.IdRoleNavigation.Name == Constants.AdministratorRoleName);

            CloseTabCmd = new(ExecuteCloseTabCmd);
        }

        public RelayCommand CloseTabCmd { get; init; }

        void ExecuteCloseTabCmd(object? obj)
        {
            if (obj is not TabItem tab)
            {
                ProgramError.Show("obj is not TabItem tab");
                return;
            }

            VMInteractionsManager.OnTabSelected(MainWindow.MainWindowTab.CatalogueTabAU);
            tab.Visibility = Visibility.Collapsed;
        }
    }
}
