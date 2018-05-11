using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public Guid InitiatorUserProfileId { get; set; }
        public Guid TargetUserProfileId { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Used { get; set; }

    }
}