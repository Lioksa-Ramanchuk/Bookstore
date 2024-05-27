using LibApp.Models;
using LibApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class OrdersRepository : IRepository<Order, long>
    {
        readonly BookstoreContext _db;
        public OrdersRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Order> GetAll() => _db.Orders.Include(o => o.IdStatusNavigation).AsQueryable();
        public Order? Get(long id) => GetAll().FirstOrDefault(o => o.Id == id);
        public async Task<Order?> GetAsync(long id) => await GetAll().FirstOrDefaultAsync(o => o.Id == id);
        public void Create(Order entity) => _db.Orders.Add(entity);
        public void Update(Order entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Order entity) => _db.Orders.Remove(entity);

        public IQueryable<Order> GetByUserId(long userId)
        {
            return GetAll().Where(o => o.IdClient == userId);
        }
        public async Task UpdateOrderStatus(long orderId, short statusId)
        {
            var o = await GetAsync(orderId);
            if (o is null)
            {
                return;
            }
            o.IdStatus = statusId;
        }
    }
}