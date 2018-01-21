using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using WebApplication4.DAL.Interfaces;
using WebApplication4.Models;

namespace WebApplication4.DAL
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private ApplicationDbContext dbContext;

        public LikeRepository(ObjectContext context) : base(context)
        {
            var newContext = new ApplicationDbContext();
            var adapter = (IObjectContextAdapter)newContext;
            var objectContext = adapter.ObjectContext;
            dbContext = newContext;
            context = objectContext;
        }


        public Like GetLikeByID(Guid id)
        {
            return dbContext.Like.Find(id);
        }

        public void UpdateLike(Like Like)
        {
            dbContext.Entry(Like).State = EntityState.Modified;
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