using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        void Save();
        void Dispose();
        IGenericRepository<ApplicationUser> UserRepository { get; }
        IGenericRepository<UserProfile> UserProfileRepository { get; }
        IGenericRepository<Post> PostRepository { get; }
        IGenericRepository<Like> LikeRepository { get; }
    }
}
