using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TableDependency;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using WebApplication4.DAL;
using WebApplication4.Hubs;
using WebApplication4.Models;
using WebApplication4.Services;

namespace WebApplication4.SqlTableDependencyClasses
{
    public class NotificationWatcher
    {
        // Singleton instance
        private static readonly  Lazy<NotificationWatcher> _instance = new Lazy<NotificationWatcher>(
            () => new NotificationWatcher(GlobalHost.ConnectionManager.GetHubContext<LikeWatcherHub>().Clients));


        private static SqlTableDependency<Like> _likesTableDependency;
        private static SqlTableDependency<Post> _postsTableDependency;
        private static SqlTableDependency<FriendRequest> _friendRequestsTableDependency;

        private NotificationWatcher(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            //likes watcher
        
            _likesTableDependency = new SqlTableDependency<Like>(
                ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString,
                "Likes",null,null,null,DmlTriggerType.All,true);

            _likesTableDependency.OnChanged += SqlLikesTableDependencyChanged;
            _likesTableDependency.OnError += SqlLikesTableDependencyOnError;
            _likesTableDependency.Start();
            
            //posts watcher
           
            _postsTableDependency = new SqlTableDependency<Post>(
                ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString,
                "Posts",null,null,null,DmlTriggerType.All,true);

            _postsTableDependency.OnChanged += SqlPostsTableDependencyChanged;
            _postsTableDependency.OnError += SqlLikesTableDependencyOnError;
            _postsTableDependency.Start();

            // friend requests watcher

            _friendRequestsTableDependency = new SqlTableDependency<FriendRequest>(
                ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString,
                "FriendRequests", null, null, null, DmlTriggerType.All, true);

            _friendRequestsTableDependency.OnChanged += SqlFriendRequestsTableDependencyChanged;
            _friendRequestsTableDependency.OnError += SqlLikesTableDependencyOnError;
            _friendRequestsTableDependency.Start();

        }

        public static NotificationWatcher Instance
        {
            get
            {
                return _instance.Value;
            } 
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public IEnumerable<Like> GetAllLikes()
        {
            var LikeModel = new List<Like>();

            var connectionString = ConfigurationManager.ConnectionStrings
                    ["connectionString"].ConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "exec SQLDependency_Posts_Likes";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        //while (sqlDataReader.Read())
                        //{
                        //    var code = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Code"));
                        //    var name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"));
                        //    var price = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Price"));

                        //    LikeModel.Add(new Like { Symbol = code, Name = name, Price = price });
                        //}
                    }
                }
            }

            return LikeModel;
        }

        void SqlLikesTableDependencyOnError(object sender, ErrorEventArgs e)
        {
            throw e.Error;
        }

        /// <summary>
        /// Broadcast New Like Price
        /// </summary>
        void SqlLikesTableDependencyChanged(object sender, RecordChangedEventArgs<Like> e)
        {
            if (e.ChangeType == ChangeType.Insert || e.ChangeType == ChangeType.Update)
            {
                var userProfileActor = UserProfileService.GetUserProfile(e.Entity.UserProfileId);
                var post = PostService.GetPost(e.Entity.PostId);
                var userProfileTarget = UserProfileService.GetUserProfile(post.UserProfile.Id);
                Notification notification = null;
                if(e.Entity.Value == 1 ) {
                    notification = NotificationService.AddNotification(userProfileTarget.Id, userProfileActor.Name + " liked your post",
                    "", userProfileActor.Name + " liked your post. Click to see your post", "/posts/" + post.Id);
                }
                else
                {
                    notification = NotificationService.AddNotification(userProfileTarget.Id, userProfileActor.Name + " disliked your post",
                        "", userProfileActor.Name + " disliked your post. Click to see your post", "/posts/" + post.Id);
                }
                if(notification != null)
                {
                    Clients.All.PushNotification(notification, userProfileTarget.Id);
                }
            }
        }

        void SqlFriendRequestsTableDependencyChanged(object sender, RecordChangedEventArgs<FriendRequest> e)
        {
            if (e.ChangeType == ChangeType.Insert)
            {
                var userProfileActor = UserProfileService.GetUserProfile(e.Entity.InitiatorUserProfileId);
                var friendRequest = FriendRequestService.GetFriendRequest(e.Entity.Id);
                var userProfileTarget = UserProfileService.GetUserProfile(friendRequest.TargetUserProfileId);
                Notification notification = null;

                notification = NotificationService.AddFriendRequestNotification(userProfileTarget.Id, userProfileActor.Name + " wants to be friends.",
                "", userProfileActor.Name + " send you a friend request. Click to see your post", "/friend-requests/" + friendRequest.Id);

                if (notification != null)
                {
                    Clients.All.PushNotification(notification, userProfileTarget.Id);
                }
            }
        }

        void SqlPostsTableDependencyChanged(object sender, RecordChangedEventArgs<Post> e)
        {
            if (e.ChangeType == ChangeType.Insert || e.ChangeType == ChangeType.Update)
            {
                 var userProfileActor = UserProfileService.GetUserProfile(e.Entity.UserProfileId);
                var post = PostService.GetPost(e.Entity.Id);
                if (post.ParentPost == null)
                {
                    return;
                }
                var parentPost = PostService.GetPost(post.ParentPost.Id);
                var userProfileTarget = UserProfileService.GetUserProfile(parentPost.UserProfile.Id);
                NotificationService.AddNotification(userProfileTarget.Id, userProfileActor.Name + " commented your post",
                    "", userProfileActor.Name + " commented your post. Click to see the post", "/posts/" + parentPost.Id);
                
            }
        }

        ~NotificationWatcher()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _likesTableDependency.Stop();
                    _friendRequestsTableDependency.Stop();
                    _postsTableDependency.Stop();
                }

                disposedValue = true;
            }
        }

       
       
        #endregion
    }
}