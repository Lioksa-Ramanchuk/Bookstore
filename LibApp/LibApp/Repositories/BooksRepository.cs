using LibApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace LibApp.Repositories
{
    public class BooksRepository : IRepository<Book, long>
    {
        readonly BookstoreContext _db;
        public BooksRepository(BookstoreContext ctxt) => _db = ctxt;

        public IQueryable<Book> GetAll() =>_db.Books.AsQueryable();
        public Book? Get(long id) => _db.Books.Find(id);
        public async Task<Book?> GetAsync(long id) => await _db.Books.FindAsync(id);
        public void Create(Book entity) => _db.Books.Add(entity);
        public void Update(Book entity)
        {
            var oldEntity = Get(entity.Id);
            if (oldEntity is null)
            {
                return;
            }

            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
        }
        public void Delete(Book entity) => _db.Books.Remove(entity);
    }
}