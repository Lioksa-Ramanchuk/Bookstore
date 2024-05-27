using LibApp.Models;
using LibApp.Repositories;
using LibApp.Views;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibApp.ViewModels
{
    public class AuthVM : BaseVM
    {
        readonly AuthWindow _authWindow;
        string _username = string.Empty;
        string _errorMsg = string.Empty;

        public AuthVM(AuthWindow authWindow)
        {
            _authWindow = authWindow;

            LoginCmd = new(ExecuteLoginCmd);
            RegisterCmd = new(ExecuteRegisterCmd);
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

        public RelayCommand LoginCmd { get; init; }
        public RelayCommand RegisterCmd { get; init; }

        void ExecuteLoginCmd(object? obj)
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
                Login? login = Task.Run(() => db.Logins.GetByNameAsync(Username)).Result;
                if (login is not null &&
                    MD5.HashData(Encoding.UTF8.GetBytes(pb.Password)).SequenceEqual(login.Password))
                {
                    var mainWindow = new MainWindow(login)
                    {
                        Owner = _authWindow.Owner,
                    };
                    mainWindow.Show();
                    VMInteractionsManager.OnAuthWindowClosing();
                }
                else
                {
                    ErrorMsg = "Няправільны лагін або пароль";
                }
            }
        }
        void ExecuteRegisterCmd(object? obj)
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
                Login? login = Task.Run(() => db.Logins.GetByNameAsync(Username)).Result;
                if (Task.Run(() => db.Logins.GetByNameAsync(Username)).Result is not null)
                {
                    ErrorMsg = "Такі лагін ужо ёсць";
                }
                else
                {
                    login = new()
                    {
                        Name = Username,
                        Password = MD5.HashData(Encoding.UTF8.GetBytes(pb.Password)),
                        IdRole = Task.Run(() => db.Roles.GetByNameAsync(Constants.UserRoleName)).Result!.Id,
                    };

                    using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
                    try 
                    {
                        db.Logins.Create(login);
                        Task.Run(db.SaveAsync).Wait();
                        Task.Run(() => tran.CommitAsync()).Wait();
                        var mainWindow = new MainWindow(login)
                        {
                            Owner = _authWindow.Owner,
                        };
                        mainWindow.Show();
                        VMInteractionsManager.OnAuthWindowClosing();
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => tran.RollbackAsync()).Wait();
                        ProgramError.Show($"{ex}");
                    }
                }
            }
        }
    }
}
