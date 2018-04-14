using System;
using System.ComponentModel.Design;
using System.Linq;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class FriendsService
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public FriendsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddFriends(Friends friends)
        {
            _context.Friends.Add(friends);
            _context.SaveChanges();
        }

        public void RemoveFriend(Guid userProfileId,Guid friendUserProfileId)
        {
            var userProfile = _context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var friendToBeRemoved = _context.UserProfile.FirstOrDefault(up => up.Id == friendUserProfileId);
            var friends = _context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Remove(friendToBeRemoved);
            _context.SaveChanges();

        }
        public void AddFriend(Guid userProfileId,Guid friendUserProfileId)
        {
            var userProfile = _context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var friendToBeAdded = _context.UserProfile.FirstOrDefault(up => up.Id == friendUserProfileId);
            var friends = _context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Add(friendToBeAdded);
            _context.SaveChanges();

        }
    }
}