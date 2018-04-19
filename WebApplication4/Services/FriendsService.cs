using System;
using System.ComponentModel.Design;
using System.Linq;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class FriendsService : BaseService
    {
        public FriendsService()
        {

        }
        public static void AddFriends(Friends friends)
        {
            Context.Friends.Add(friends);
            Context.SaveChanges();
        }

        public static void RemoveFriend(Guid userProfileId,Guid friendUserProfileId)
        {
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var friendToBeRemoved = Context.UserProfile.FirstOrDefault(up => up.Id == friendUserProfileId);
            var friends = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Remove(friendToBeRemoved);
            Context.SaveChanges();

        }
        public static void AddFriend(Guid userProfileId,Guid friendUserProfileId)
        {
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var friendToBeAdded = Context.UserProfile.FirstOrDefault(up => up.Id == friendUserProfileId);
            var friends = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Add(friendToBeAdded);
            Context.SaveChanges();

        }
    }
}