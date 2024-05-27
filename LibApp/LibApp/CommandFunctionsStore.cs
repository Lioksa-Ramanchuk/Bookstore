using LibApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApp
{
    public static class CommandFunctionsStore
    {
        public static bool AddToFavouriteCmdFunc(long userId, long bookId)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                db.Favourites.Create(userId, bookId);
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return false;
            }
            return true;
        }
        public static bool RemoveFromFavouriteCmdFunc(long userId, long bookId)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                db.Favourites.Delete(userId, bookId);
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return false;
            }
            return true;
        }
        public static bool AddToCartCmdFunc(long userId, long bookId)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                db.Carts.Create(userId, bookId);
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return false;
            }
            return true;
        }
        public static bool RemoveFromCartCmdFunc(long userId, long bookId)
        {
            using var db = new DbUnit();
            using var tran = Task.Run(() => db.BeginTransactionAsync()).Result;
            try
            {
                db.Carts.Delete(userId, bookId);
                Task.Run(db.SaveAsync).Wait();
                Task.Run(() => tran.CommitAsync()).Wait();
            }
            catch (Exception ex)
            {
                Task.Run(() => tran.RollbackAsync()).Wait();
                ProgramError.Show($"{ex}");
                return false;
            }
            return true;
        }
    }
}
