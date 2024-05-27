using LibApp.Models;
using LibApp.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class OrderStatusesRepository : IRepository<OrderStatus, short>
    {
        readonly BookstoreContext _db;
        public OrderStatusesRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<OrderStatus> GetAll() => _db.OrderStatuses.AsQueryable();
        public OrderStatus? Get(short id) => _db.OrderStatuses.Find(id);
        public async Task<OrderStatus?> GetAsync(short id) => await _db.OrderStatuses.FindAsync(id);
        public void Create(OrderStatus entity) => _db.OrderStatuses.Add(entity);
        public void Update(OrderStatus entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(OrderStatus entity) => _db.OrderStatuses.Remove(entity);

        public OrderStatus? GetByName(string name)
        {
            return GetAll().FirstOrDefault(os => os.Name == name);
        }
    }
}
