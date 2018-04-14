using System;
using WebApplication4.DAL;
using WebApplication4.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;


namespace WebApplication4.Services
{
    public class UserProfileService
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public UserProfileService(ApplicationDbContext context)
        {
            _context = context;
        }
        public UserProfileService()
        {

        }
        public List<UserProfile> GetAllUserProfiles()
        {
            var result = _context.UserProfile.Include(a => a.User).ToList();
            return result;
        }
        public UserProfile GetUserProfile(Guid id)
        {
            var result1 = _context.UserProfile.Include(up => up.User).ToList();
            
            var result = _context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.Id == id);
            return result;
        }
        
        public UserProfile GetUserProfileTest(Guid id)
        {
            
            var result = _context.UserProfile.FirstOrDefault(up => up.Id == id);
            return result;
        }
        public UserProfile GetUserProfileByUserId(Guid id)
        {
            var result = _context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.User.Id == id.ToString());

            return result;
        }
        
        public UserProfile GetUserProfileByUserAddress(string userAddress)
        {
            var result = _context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.UserAddress == userAddress);

            return result;
        }

        public Friends GetUserProfileFriends(Guid id)

        {
            var result = _context.Friends.FirstOrDefault(up => up.UserProfile.Id == id);

            return result;
        }

        public List<UserProfile> SearchUserProfilesByUserName(string searchString)
        {
            var userProfiles = _context.UserProfile
                .Include(up => up.User)
                .Where(x => x.Name.Contains(searchString))
                .Select(x => new UserProfile(){ UserAddress = x.UserAddress, Name = x.Name }).ToList();
            return userProfiles;
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            
            _context.Entry(userProfile).State = userProfile.Id == Guid.Empty? EntityState.Added : EntityState.Modified;

            _context.SaveChanges();
        }

        public void InsertUserProfile(UserProfile userProfile)
        {
            _context.UserProfile.Add(userProfile);
            _context.SaveChanges();
        }
        public void DeleteUserProfile(Guid id)

        {
            var userProfile = GetUserProfileByUserId(id);
            _context.UserProfile.Remove(userProfile);
            _context.SaveChanges();
        }
    }
}