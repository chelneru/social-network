using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApplication4.DAL;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class NotificationService : BaseService
    {

        public NotificationService()
        {
            
        }
        public static bool AddNotification(Guid userTargetId, string title, string icon, string message, string link)
        {
            var notification = new Notification
            {
                Link                = link,
                NotificationIcon =   icon,
                NotificationMessage = message,
                NotificationTime = DateTime.Now,
                NotificationTitle = title,
                UserId = userTargetId,
                Seen = false
            };
       Context.Notification.Add(notification);
            Context.SaveChanges();
            return true;
        }

        public static List<Notification> GetNewNotifications(Guid userProfileId)
        {
            var notifications = Context.Notification
                .Where(n => n.UserId == userProfileId && n.Seen == false)
                .ToList();
            return notifications;
        }
    }
}