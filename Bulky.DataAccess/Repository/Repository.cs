using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        public Repository(AppDbContext db)
        {
            _db = db;
        }
        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            var query = _db.Set<T>().Where(filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public void Remove(T enttiy)
        {
            _db.Set<T>().Remove(enttiy);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _db.RemoveRange(entity);
        }
    }
}
