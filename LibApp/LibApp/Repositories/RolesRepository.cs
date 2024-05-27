using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class RolesRepository : IRepository<Role, short>
    {
        readonly BookstoreContext _db;
        public RolesRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Role> GetAll() => _db.Roles.AsQueryable();
        public Role? Get(short id) => _db.Roles.Find(id);
        public async Task<Role?> GetAsync(short id) => await _db.Roles.FindAsync(id);
        public void Create(Role entity) => _db.Roles.Add(entity);
        public void Update(Role entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Role entity) => _db.Roles.Remove(entity);

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await GetAll().Where(r => r.Name.Equals(name)).FirstOrDefaultAsync();
        }
    }
}
