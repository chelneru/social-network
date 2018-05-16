using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using TableDependency.SqlClient;
using WebApplication4.Models;
using WebApplication4.Services;
using WebApplication4.SignalIR;
using WebApplication4.SqlTableDependencyClasses;
using WebApplication4.ViewModels;

namespace WebApplication4.Hubs
{
    [HubName("NotificationWatcher")]
    public class LikeWatcherHub : Hub
    {
        private readonly NotificationWatcher _likeWatcher;
        private static readonly List<UserDetail> ConnectedUsers = new List<UserDetail>();
        private static SqlTableDependency<Notification> _tableDependency;
        private readonly NotificationService  _notificationService= new NotificationService();
        public LikeWatcherHub() :
        this(NotificationWatcher.Instance)
        {

        }
        public LikeWatcherHub(NotificationWatcher likeWatcher)
        {
            _likeWatcher = likeWatcher;
        }

        public void Connect(Guid userProfileId)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserProfileId = userProfileId });
            }
        }

        public static void  PushNotification(Notification notification, Guid userProfileId)
        {
            UserDetail currentUser = null;
            foreach (var connectedUser in ConnectedUsers)
            {
                if (connectedUser.UserProfileId == userProfileId)
                {
                    currentUser = connectedUser;
                    break;
                }
            }

            if (currentUser != null)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<LikeWatcherHub>();

                context.Clients.Client(currentUser.ConnectionId).PushNotification(notification);

            }

        }
        
        public bool MarkSeenNotifications(List<string> notifIds)
        {
            var request = Context.Request;
            _notificationService.MarkNotificationsAsSeen(notifIds);
            return true;
        }
        public NotificationsViewModel GetAllNotifications(Guid userProfileId)
        {
            var result = new NotificationsViewModel()
            {
                Notifications = _notificationService.GetNewNotifications(userProfileId),
                NotificationCount = _notificationService.GetUnseenNotificationsCount(userProfileId)
                
        };

            return result;
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                if (ConnectedUsers.Count(u => u.UserId == item.UserId) == 0)
                {
                    var id = item.UserId.ToString();
                    Clients.All.onUserDisconnected(id, item.UserName);
                }
            }
            return base.OnDisconnected(stopCalled);
        }
       

       
    }
}