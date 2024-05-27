using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class OrderedBooksRepository : IRepository<OrderedBook, long>
    {
        readonly BookstoreContext _db;
        public OrderedBooksRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<OrderedBook> GetAll() => _db.OrderedBooks.Include(ob => ob.IdBookNavigation)
                                                                   .AsQueryable();
        public OrderedBook? Get(long id) => GetAll().FirstOrDefault(ob => ob.Id == id);
        public async Task<OrderedBook?> GetAsync(long id) => await GetAll().FirstOrDefaultAsync(ob => ob.Id == id);
        public void Create(OrderedBook entity) => _db.OrderedBooks.Add(entity);
        public void Update(OrderedBook entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(OrderedBook entity) => _db.OrderedBooks.Remove(entity);
        public async Task OrderCartAsync(long userId, long orderId)
        {
            var orderedBooks = from c in _db.Carts
                               where c.IdClient == userId
                               join b in _db.Books on c.IdBook equals b.Id
                               select new OrderedBook
                               {
                                   IdOrder = orderId,
                                   IdBook = c.IdBook,
                                   Count = c.Count,
                                   Price = b.Price,
                               };

            await _db.OrderedBooks.BulkInsertAsync(orderedBooks);
        }
        public IQueryable<OrderedBook> GetWhereNotEnough(long orderId)
        {
            return GetAll().Where(ob => ob.IdOrder == orderId && ob.Count > ob.IdBookNavigation.Count);
        }
        public decimal GetOrderCostByOrderId(long orderId)
        {
            return GetAll().Where(ob => ob.IdOrder == orderId).Select(ob => ob.Price * ob.Count).Sum();
        }
    }
}