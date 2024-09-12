using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }

        public IShoppingCartRepository shoppingCartRepository { get; private set; }

        public IApplicationUserRepository applicationUserRepository { get; private set; }
        public IOrderHeaderRepo orderHeaderRepo { get; private set; }
        public IOrderDetailRepo orderDetailRepo { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ProductRepository = new ProductRepository(_db);
            shoppingCartRepository = new ShoppingCartRepository(_db);
            applicationUserRepository = new ApplicationUserRepository(_db);
            orderHeaderRepo = new OrderHeaderRepo(_db);
            orderDetailRepo = new OrderDetailRepo(_db);
        }

       


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
