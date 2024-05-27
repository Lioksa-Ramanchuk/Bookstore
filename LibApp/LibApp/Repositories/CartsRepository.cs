using LibApp.Models;
using LibApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibApp.Repositories
{
    public class CartsRepository : IRepository<Cart, long>
    {
        readonly BookstoreContext _db;
        public CartsRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Cart> GetAll() => _db.Carts.Include(c => c.IdBookNavigation).AsQueryable();
        public Cart? Get(long id) => GetAll().FirstOrDefault(c => c.Id == id);
        public async Task<Cart?> GetAsync(long id) => await GetAll().FirstOrDefaultAsync(c => c.Id == id);
        public void Create(Cart entity) => _db.Carts.Add(entity);
        public void Create(long userId, long bookId, int count = 1)
        {
            Create(new() { IdClient = userId, IdBook = bookId, Count = count });
        }
        public void Update(Cart entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Cart entity) => _db.Carts.Remove(entity);
        public void Delete(long userId, long bookId)
        {
            if (GetAll().FirstOrDefault(c => c.IdClient == userId && c.IdBook == bookId) is Cart c)
            {
                Delete(c);
            }
        }

        public decimal GetCostByUserId(long userId)
        {
            return GetAll().Where(c => c.IdClient == userId)
                .Select(c => c.IdBookNavigation.Price * c.Count).Sum();
        }
        public IQueryable<Book> GetBooksByUserId(long userId)
        {
            return GetAll().Where(c => c.IdClient == userId)
                .Select(c => c.IdBookNavigation);
        }
        public async Task<int> DeleteByUserIdAsync(long userId)
        {
            return await _db.Carts.Where(f => f.IdClient == userId).ExecuteDeleteAsync();
        }
        public IQueryable<Cart> GetWhereNotEnough(long userId)
        {
            return GetAll().Where(c => c.IdClient == userId && c.Count > c.IdBookNavigation.Count);
        }
        public bool IsInCart(long userId, long bookId)
        {
            return GetAll().Any(c => c.IdClient == userId && c.IdBook == bookId);
        }
    }
}
