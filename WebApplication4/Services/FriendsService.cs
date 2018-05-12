using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class FriendsService : BaseService
    {
        public FriendsService()
        {
        }


        public static Friends CreateEmptyFriendsEntity(UserProfile up)
        {
            if (up != null)
            {
                var friendsEntity = new Friends
                {
                    UserProfile = up
                };
                Context.Friends.Add(friendsEntity);
                Context.SaveChanges();
                return friendsEntity;
            }

            return null;
        }
        public static void AddFriends(Friends friends)
        {
            Context.Friends.Add(friends);
            Context.SaveChanges();
        }

        public static void RemoveFriend(Guid userProfileId, Guid userProfileId2)
        {
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var userProfile2 = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId2);

            var friends = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Remove(userProfile2);
            //remove friend both ways
            var friends2 = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile2.Id);
            friends2?.Friend_UserProfile.Remove(userProfile);

            Context.SaveChanges();
        }

        public static void AddFriend(Guid userProfileId, Guid userProfileId2)
        {
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            var userProfile2 = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId2);

            var friends = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile.Id);
            friends?.Friend_UserProfile.Add(userProfile2);
            //add friend both ways

            var friends2 = Context.Friends.FirstOrDefault(fr => fr.UserProfile.Id == userProfile2.Id);
            friends2?.Friend_UserProfile.Add(userProfile);

            Context.SaveChanges();
        }

        public static void AddFriendToFriendEntity(Guid friendEntityId, Guid userProfileId)
        {
            var friendEntity = Context.Friends.Include(a => a.UserProfile)
                .FirstOrDefault(frnd => frnd.Id == friendEntityId);
            var userProfile = Context.UserProfile.FirstOrDefault(up => up.Id == userProfileId);
            if (userProfile != null && friendEntity != null)
            {
                //add userProfile to the friendEntity friends list
                if (friendEntity.Friend_UserProfile == null)
                {
                    friendEntity.Friend_UserProfile = new List<UserProfile> {userProfile};
                }
                else
                {
                    friendEntity.Friend_UserProfile.Add(userProfile);
                }

                //add friendEntity's userProfile to the userProfile's friendEntity list
                var userProfileFriendEntity =
                    Context.Friends.FirstOrDefault(frnd => frnd.UserProfile.Id == userProfileId);
                if (userProfileFriendEntity != null)
                {
                    if (userProfileFriendEntity.Friend_UserProfile != null)
                    {
                        userProfileFriendEntity.Friend_UserProfile.Add(friendEntity.UserProfile);
                    }
                    else
                    {
                        userProfileFriendEntity.Friend_UserProfile = new List<UserProfile> {friendEntity.UserProfile};
                    }

                    Context.SaveChanges();
                }
            }
        }

        public static bool CheckFriendship(Guid userProfileId1, Guid userProfileId2)
        {
            var friendsList = UserProfileService.GetUserProfileFriends(userProfileId1);
            if (friendsList == null)
            {
                return false;
            }

            var friend = friendsList.Friend_UserProfile.FirstOrDefault(up => up.Id == userProfileId2);
            if (friend == null)
            {
                return false;
            }

            return true;
        }
    }
}