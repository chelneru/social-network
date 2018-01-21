using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using WebApplication4.DAL;
using WebApplication4.Models;

namespace WebApplication4.DAL
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private ApplicationDbContext dbContext;

        public UserRepository(ObjectContext context) : base(context)
        {
            var newContext = new ApplicationDbContext();
            var adapter = (IObjectContextAdapter)newContext;
            var objectContext = adapter.ObjectContext;
            context = objectContext;
        }

        public ApplicationUser GetUserByID(string id)
        {
            return dbContext.Users.Find(id);
        }

        public void UpdateUser(ApplicationUser user)
        {
            dbContext.Entry(user).State = EntityState.Modified;
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        private bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}