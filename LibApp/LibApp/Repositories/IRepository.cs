using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApp.Repositories
{
    public interface IRepository<T, K> where T : class
    {
        IQueryable<T> GetAll();
        T? Get(K id);
        Task<T?> GetAsync(K id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
