using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class NotificationsViewModel
    {
        public List<Notification> Notifications { get; set; }
        public Int16 NotificationCount { get; set; }
    }
}