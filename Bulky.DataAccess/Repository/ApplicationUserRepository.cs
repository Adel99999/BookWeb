using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUsers> , IApplicationUserRepository
    {
        private readonly AppDbContext _db;
        public ApplicationUserRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
    }
}
