using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public class LoginsRepository : IRepository<Login, long>
    {
        readonly BookstoreContext _db;
        public LoginsRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Login> GetAll() => _db.Logins.Include(l => l.IdRoleNavigation).AsQueryable();
        public Login? Get(long id) => GetAll().FirstOrDefault(l => l.Id == id);
        public async Task<Login?> GetAsync(long id) => await GetAll().FirstOrDefaultAsync(l => l.Id == id);
        public void Create(Login entity) => _db.Logins.Add(entity);
        public void Update(Login entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Login entity) => _db.Logins.Remove(entity);

        public async Task UpdateAsync(Login entity)
        {
            var oldEntity = await GetAsync(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public async Task<Login?> GetByNameAsync(string name)
        {
            return await GetAll().Where(l => l.Name.Equals(name)).FirstOrDefaultAsync();
        }
        public IQueryable<Login> GetAdmins()
        {
            return GetAll().Where(l => l.IdRoleNavigation.Name == Constants.AdministratorRoleName);
        }
    }
}
