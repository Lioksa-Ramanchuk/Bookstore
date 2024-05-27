using LibApp.Models;
using LibApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibApp.Repositories
{
    public class FavouritesRepository : IRepository<Favourite, long>
    {
        readonly BookstoreContext _db;
        public FavouritesRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Favourite> GetAll() => _db.Favourites.Include(c => c.IdBookNavigation).AsQueryable();
        public Favourite? Get(long id) => GetAll().FirstOrDefault(f => f.Id == id);
        public async Task<Favourite?> GetAsync(long id) => await GetAll().FirstOrDefaultAsync(f => f.Id == id);
        public void Create(Favourite entity) => _db.Favourites.Add(entity);
        public void Create(long userId, long bookId)
        {
            Create(new() { IdClient = userId, IdBook = bookId });
        }
        public void Update(Favourite entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Favourite entity) => _db.Favourites.Remove(entity);
        public void Delete(long userId, long bookId)
        {
            if (GetAll().FirstOrDefault(f => f.IdClient == userId && f.IdBook == bookId) is Favourite f)
            {
                Delete(f);
            }
        }

        public void AddAllToCartByUserId(long userId)
        {
            _db.Carts.AddRange(
                _db.Favourites
                   .Where(f => f.IdClient == userId)
                   .Select(f => f.IdBook)
                .Except(_db.Carts
                   .Where(c => c.IdClient == userId)
                   .Select(c => c.IdBook))
                .Select(bookId => new Cart() { IdClient = userId, IdBook = bookId, Count = 1 }));
        }
        public IQueryable<Book> GetByUserId(long userId)
        {
            return GetAll().Where(f => f.IdClient == userId)
                .Select(f => f.IdBookNavigation);
        }
        public async Task<int> DeleteByUserIdAsync(long userId)
        {
            return await _db.Favourites.Where(f => f.IdClient == userId).ExecuteDeleteAsync();
        }
        public bool IsFavourite(long userId, long bookId)
        {
            return GetAll().Any(f => f.IdClient == userId && f.IdBook == bookId);
        }
    }
}
