using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            // Include the Category navigation property if T is Product
            if (typeof(T) == typeof(Product))
            {
                query = query.Include(p => ((Product)(object)p).category);
            }

            query = query.Where(filter);
            return query.FirstOrDefault();
        }
        

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;

            // Include the Category navigation property if T is Product
            if (typeof(T) == typeof(Product))
            {
                query = query.Include(p => ((Product)(object)p).category);
            }

            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
