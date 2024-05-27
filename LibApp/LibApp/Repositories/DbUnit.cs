using LibApp.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class DbUnit : IDisposable
    {
        readonly BookstoreContext _db = new();
        BooksRepository? _booksRepository;
        CartsRepository? _cartsRepository;
        FavouritesRepository? _favouritesRepository;
        LoginsRepository? _loginsRepository;
        OrderedBooksRepository? _orderedBooksRepository;
        OrdersRepository? _ordersRepository;
        OrderStatusesRepository? _orderStatusesRepository;
        RolesRepository? _rolesRepository;
        bool disposedValue;

        public BooksRepository Books => _booksRepository ??= new(_db);
        public CartsRepository Carts => _cartsRepository ??= new(_db);
        public FavouritesRepository Favourites => _favouritesRepository ??= new(_db);
        public LoginsRepository Logins => _loginsRepository ??= new(_db);
        public OrderedBooksRepository OrderedBooks => _orderedBooksRepository ??= new(_db);
        public OrdersRepository Orders => _ordersRepository ??= new(_db);
        public OrderStatusesRepository OrderStatuses => _orderStatusesRepository ??= new(_db);
        public RolesRepository Roles => _rolesRepository ??= new(_db);

        public void Save() => _db.SaveChanges();
        public async Task SaveAsync() => await _db.SaveChangesAsync();
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _db.Database.BeginTransactionAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _db?.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
