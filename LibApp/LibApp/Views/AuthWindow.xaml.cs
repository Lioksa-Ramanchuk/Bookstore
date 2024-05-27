using LibApp.Models;
using LibApp.Repositories;
using LibApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibApp.Views
{
    public partial class AuthWindow : Window
    {
        static AuthWindow()
        {
            InitDb();
        }
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthVM(this);

            VMInteractionsManager.AuthWindowClosing += CloseWindow;
        }

        void CloseWindow()
        {
            VMInteractionsManager.AuthWindowClosing -= CloseWindow;
            Close();
        }

        static void InitDb()
        {
            using var db = new DbUnit();

            using var tranAddRoles = Task.Run(db.BeginTransactionAsync).Result;
            try
            {
                if (Task.Run(() => db.Roles.GetByNameAsync(Constants.AdministratorRoleName)).Result is null)
                {
                    db.Roles.Create(new Role() { Name = Constants.AdministratorRoleName });
                }
                if (Task.Run(() => db.Roles.GetByNameAsync(Constants.UserRoleName)).Result is null)
                {
                    db.Roles.Create(new Role() { Name = Constants.UserRoleName });
                }
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tranAddRoles.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tranAddRoles.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
            }

            using var tranAddAdmin = Task.Run(db.BeginTransactionAsync).Result;
            try
            {
                if (Task.Run(() => db.Logins.GetAdmins().CountAsync()).Result == 0)
                {
                    var AdminRoleId = Task.Run(() => db.Roles.GetByNameAsync(Constants.AdministratorRoleName)).Result!.Id;
                    if (Task.Run(() => db.Logins.GetByNameAsync(Constants.DefaultAdministratorLogin)).Result is Login l)
                    {
                        Task.Run(() => db.Logins.UpdateAsync(new Login()
                        {
                            Name = l.Name,
                            Password = l.Password,
                            IdRole = AdminRoleId,
                        })).Wait();
                    }
                    else
                    {
                        db.Logins.Create(new Login()
                        {
                            Name = Constants.DefaultAdministratorLogin,
                            Password = MD5.HashData(Encoding.UTF8.GetBytes(Constants.DefaultAdministratorPassword)),
                            IdRole = AdminRoleId,
                        });
                    }
                }
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tranAddAdmin.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tranAddAdmin.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
            }

            using var tranAddOrderStatuses = Task.Run(db.BeginTransactionAsync).Result;
            try
            {
                if (db.OrderStatuses.GetByName(Constants.ProcessingOrderStatusName) is null)
                {
                    db.OrderStatuses.Create(new OrderStatus() { Name = Constants.ProcessingOrderStatusName });
                }
                if (db.OrderStatuses.GetByName(Constants.ShippedOrderStatusName) is null)
                {
                    db.OrderStatuses.Create(new OrderStatus() { Name = Constants.ShippedOrderStatusName });
                }
                if (db.OrderStatuses.GetByName(Constants.DeliveredOrderStatusName) is null)
                {
                    db.OrderStatuses.Create(new OrderStatus() { Name = Constants.DeliveredOrderStatusName });
                }
                if (db.OrderStatuses.GetByName(Constants.CanceledOrderStatusName) is null)
                {
                    db.OrderStatuses.Create(new OrderStatus() { Name = Constants.CanceledOrderStatusName });
                }
                if (db.OrderStatuses.GetByName(Constants.ReturnedOrderStatusName) is null)
                {
                    db.OrderStatuses.Create(new OrderStatus() { Name = Constants.ReturnedOrderStatusName });
                }
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tranAddOrderStatuses.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tranAddOrderStatuses.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
            }
        }
    }
}
