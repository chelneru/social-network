﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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
        public static Notification AddNotification(Guid userTargetId, string title, string icon, string message, string link)
        {
            var notification = new Notification
            {
                Link                = link,
                NotificationIcon =   icon,
                NotificationMessage = message,
                NotificationTime = DateTime.Now,
                NotificationTitle = title,
                isRequest = false,
                UserId = userTargetId,
                Seen = false
            };
       Context.Notification.Add(notification);
            Context.SaveChanges();
            return notification;
        }

        public static Notification AddFriendRequestNotification(Guid userTargetId, string title, string icon, string message, string link)
        {
            var notification = new Notification
            {
                Link = link,
                NotificationIcon = icon,
                NotificationMessage = message,
                NotificationTime = DateTime.Now,
                NotificationTitle = title,
                isRequest = true,
                UserId = userTargetId,
                Seen = false
            };
            Context.Notification.Add(notification);
            Context.SaveChanges();
            return notification;
        }

        public static void MarkNotificationsAsSeen(List<string> notificationsIds)
        {
            var notificationEntities = Context.Notification
                .Where(n=> notificationsIds.Any(nt=> n.Id.ToString().Contains(nt)))
                .ToList();

            foreach(Notification notification in notificationEntities)
            {
                notification.Seen = true;
            }
            Context.SaveChanges();
        }
        public static Int16 GetUnseenNotificationsCount(Guid userProfileId)
        {
            var result = (Int16)Context.Notification.Count(nt => nt.Seen == false && nt.UserId == userProfileId && nt.isRequest == false);
            return result;
        }
        public static List<Notification> GetAllNotifications(Guid userProfileId)
        {
            //Context.Notification.Re()
            var result = Context.Notification
                .Where(n => n.UserId == userProfileId )
                .OrderBy(n => n.NotificationTime)
                .AsNoTracking()
                .ToList();
            return result;
        }
        public static void DeleteAllNotifications(Guid userProfileId)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Notifications WHERE UserId = {0} AND isRequest = false", userProfileId);
            }
        }
        public static void DeleteNotification(Int16 notifId)
        {
            var notification = Context.Notification.FirstOrDefault(nt => nt.Id == notifId);
            Context.Notification.Remove(notification);
            Context.SaveChanges();
        }

        public static List<Notification> GetNewNotifications(Guid userProfileId)
        {
            var notifications = Context.Notification
                .Where(n => n.UserId == userProfileId && n.Seen == false)
                .OrderBy(n => n.NotificationTime)
                .AsNoTracking()
                .Take(10)
                .ToList();
            return notifications;
        }
    }
}