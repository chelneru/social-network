using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication4.DAL.Interfaces;


namespace WebApplication4.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
   
        public virtual UserProfile UserProfile { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<Post> Post { get; set; }
        public IDbSet<Like> Like { get; set; }
        public IDbSet<Notification> Notification { get; set; }
        public IDbSet<FriendRequest> FriendRequest { get; set; }
        public IDbSet<PrivateMessage> PrivateMessage { get; set; }
        public IDbSet<LinkPreview> LinkPreview { get; set; }

        public virtual IDbSet<UserProfile> UserProfile { get; set; }
        public IDbSet<Friends> Friends { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>()
           .HasRequired(d => d.UserProfile)
           .WithMany(l => l.Likes)
            .WillCascadeOnDelete(false); ;

            modelBuilder.Entity<UserProfile>()
                .HasRequired(b => b.User)
                .WithOptional(a => a.UserProfile)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}