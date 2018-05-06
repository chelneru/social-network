using System;
using WebApplication4.DAL;
using WebApplication4.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using WebApplication4.ViewModels;

namespace WebApplication4.Services
{
    public class UserProfileService : BaseService
    {

        //public UserProfileService(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        public UserProfileService()
        {

        }
        public static List<UserProfile> GetAllUserProfiles()
        {
            var result = Context.UserProfile.Include(a => a.User).ToList();
            return result;
        }
        public static UserProfile GetUserProfile(Guid id)
        {
            var result1 = Context.UserProfile.Include(up => up.User).ToList();
            
            var result = Context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.Id == id);
            return result;
        }
        public static void ChangeUserProfileAvatar(Guid userProfileId, String path)
        {
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            userProfile.AvatarUrl = path;
            Context.SaveChanges();
        }
        public static UserProfile GetUserProfileTest(Guid id)
        {
            
            var result = Context.UserProfile.FirstOrDefault(up => up.Id == id);
            return result;
        }
        public static UserProfile GetUserProfileByUserId(Guid id)
        {
            var result = Context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.User.Id == id.ToString());

            return result;
        }
        
        public static UserProfile GetUserProfileByUserAddress(string userAddress)
        {
            var result = Context.UserProfile.Include(up => up.User).FirstOrDefault(up => up.UserAddress == userAddress);

            return result;
        }

        public static Friends GetUserProfileFriends(Guid id)

        {
            var result = Context.Friends.FirstOrDefault(up => up.UserProfile.Id == id);

            return result;
        }

        public static List<SearchResultModel> SearchUserProfilesByUserName(string searchString)
        {
            var userProfiles = Context.UserProfile
                .Include(up => up.User)
                .Where(x => x.Name.Contains(searchString))
                .Select(x => new SearchResultModel{ Link = "/users/" + x.UserAddress, Content = x.Name }).ToList();
            return userProfiles;
        }

        public static void UpdateUserProfile(UserProfile userProfile)
        {
            
            Context.Entry(userProfile).State = userProfile.Id == Guid.Empty? EntityState.Added : EntityState.Modified;

            Context.SaveChanges();
        }

        public static void InsertUserProfile(UserProfile userProfile)
        {
            Context.UserProfile.Add(userProfile);
            Context.SaveChanges();
        }
        public static void DeleteUserProfile(Guid id)

        {
            var userProfile = GetUserProfileByUserId(id);
            Context.UserProfile.Remove(userProfile);
            Context.SaveChanges();
        }
    }
}