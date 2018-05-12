using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class UserProfileDetailsViewModel
    {
        public UserProfile Profile { get; set; }
        public Friends FriendsCollection { get; set; }
        public FriendRequest ActiveFriendRequest { get; set; }
        
    }
}