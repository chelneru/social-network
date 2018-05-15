using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationIcon { get; set; }
        public string EntityId { get; set; }
        public string NotificationMessage { get; set; }
        public string Link { get; set; }
        public bool IsRequest { get; set; }
        public DateTime NotificationTime { get; set; }
        public bool Seen { get; set; }
    }
}