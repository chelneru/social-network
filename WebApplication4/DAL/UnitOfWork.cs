using System;
using WebApplication4.DAL.Interfaces;
using WebApplication4.Models;

namespace WebApplication4.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private GenericRepository<UserProfile> userProfileRepository;
        private GenericRepository<ApplicationUser> userRepository;
        private GenericRepository<Post> postRepository;
        private GenericRepository<Like> likeRepository;
        private GenericRepository<Friends> friendsRepository;

        public IGenericRepository<ApplicationUser> UserRepository
        {
            get
            {
                return this.userRepository ?? new GenericRepository<ApplicationUser>(context);
            }
        }
        public IGenericRepository<UserProfile> UserProfileRepository
        {
            get
            {
                return this.userProfileRepository ?? new GenericRepository<UserProfile>(context);
            }
        }

        public IGenericRepository<Post> PostRepository
        {
            get
            {
                return this.postRepository ?? new GenericRepository<Post>(context);
            }
        }
        public IGenericRepository<Like> LikeRepository
        {
            get
            {
                return this.likeRepository ?? new GenericRepository<Like>(context);
            }
        }

        public IGenericRepository<Friends> FriendsRepository
        {
            get
            {
                return this.friendsRepository ?? new GenericRepository<Friends>(context);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
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