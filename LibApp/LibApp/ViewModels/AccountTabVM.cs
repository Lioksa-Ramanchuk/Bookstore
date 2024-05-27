using LibApp.Models;
using LibApp.Views;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibApp.Repositories;

namespace LibApp.ViewModels
{
    public class AccountTabVM : BaseVM
    {
        readonly MainWindow _mainWindow;
        readonly Login _currUser;
        string _username = string.Empty;
        string _errorMsg = string.Empty;

        public AccountTabVM(MainWindow mainWindow, Login user)
        {
            _mainWindow = mainWindow;
            _currUser = user;
            Username = _currUser.Name;

            UpdateLoginPasswordCmd = new(ExecuteUpdateLoginPasswordCmd);
            SignOutCmd = new(ExecuteSignOutdCmd);
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                _errorMsg = value;
                OnPropertyChanged(nameof(ErrorMsg));
            }
        }

        public RelayCommand UpdateLoginPasswordCmd { get; init; }
        public RelayCommand SignOutCmd { get; init; }

        void ExecuteUpdateLoginPasswordCmd(object? obj)
        {
            if (obj is not PasswordBox pb)
            {
                ProgramError.Show("obj is not PasswordBox");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMsg = "Увядзіце лагін";
            }
            else if (!Constants.LoginRgx.IsMatch(Username))
            {
                ErrorMsg = "Некарэктны лагін";
            }
            else if (string.IsNullOrWhiteSpace(pb.Password))
            {
                ErrorMsg = "Увядзіце пароль";
            }
            else if (pb.Password.Length < Constants.MinPasswordLength)
            {
                ErrorMsg = $"Мінімальная дапушчальная даўжыня пароля: {Constants.MinPasswordLength}";
            }
            else if (!Constants.PasswordRgx.IsMatch(pb.Password))
            {
                ErrorMsg = "Некарэктны пароль";
            }
            else
            {
                using var db = new DbUnit();

                if (Username != _currUser.Name && Task.Run(() => db.Logins.GetByNameAsync(Username)).Result is not null)
                {
                    ErrorMsg = "Такі лагін ужо ёсць";
                }
                else
                {
                    Login newLogin = new()
                    {
                        Id = _currUser.Id,
                        Name = Username,
                        Password = MD5.HashData(Encoding.UTF8.GetBytes(pb.Password)),
                        IdRole = _currUser.IdRole,
                    };

                    using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
                    try
                    {
                        Task.Run(() => db.Logins.UpdateAsync(newLogin)).Wait();
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
                    MessageBox.Show("Звесткі абноўленыя", "Акаўнт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        void ExecuteSignOutdCmd(object? obj)
        {
            var authWindow = new AuthWindow()
            {
                Owner = _mainWindow.Owner,
            };
            authWindow.Show();
            VMInteractionsManager.OnMainWindowClosing();
        }
    }
}
