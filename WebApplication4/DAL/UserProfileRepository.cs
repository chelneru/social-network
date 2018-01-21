using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.DAL
{
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        private ApplicationDbContext dbContext;

        public UserProfileRepository(ObjectContext context) : base(context)
        {
            var newContext = new ApplicationDbContext();
            var adapter = (IObjectContextAdapter)newContext;
            var objectContext = adapter.ObjectContext;
            context = objectContext;
        }

        public UserProfile GetUserProfileByID(Guid id)
        {
            return dbContext.UserProfile.Find(id);
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            dbContext.Entry(userProfile).State = EntityState.Modified;
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