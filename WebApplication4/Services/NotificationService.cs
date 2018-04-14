using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApplication4.DAL;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public NotificationService()
        {

        }
        public bool AddNotification(Guid userTargetId, string title, string icon, string message, string link)
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
       _context.Notification.Add(notification);
            _context.SaveChanges();
            return true;
        }

        public List<Notification> GetNewNotifications(Guid userProfileId)
        {
            var notifications = _context.Notification
                .Where(n => n.UserId == userProfileId && n.Seen == false)
                .ToList();
            return notifications;
        }
    }
}