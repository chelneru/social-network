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
    public class LikeWatcher
    {
        // Singleton instance
        private static readonly  Lazy<LikeWatcher> instance = new Lazy<LikeWatcher>(
            () => new LikeWatcher(GlobalHost.ConnectionManager.GetHubContext<LikeWatcherHub>().Clients));


        private static SqlTableDependency<Like> _likesTableDependency;
        private static SqlTableDependency<Post> _postsTableDependency;

        private LikeWatcher(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _likesTableDependency = new SqlTableDependency<Like>(
                ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString,
                "Likes",null,null,null,DmlTriggerType.All,true);

            _likesTableDependency.OnChanged += SqlLikesTableDependencyChanged;
            _likesTableDependency.OnError += SqlLikesTableDependencyOnError;
            _likesTableDependency.Start();
            
            
           
            _postsTableDependency = new SqlTableDependency<Post>(
                ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString,
                "Posts",null,null,null,DmlTriggerType.All,true);

            _postsTableDependency.OnChanged += SqlPostsTableDependencyChanged;
            _postsTableDependency.OnError += SqlLikesTableDependencyOnError;
            _postsTableDependency.Start();
            
            
        }

        public static LikeWatcher Instance
        {
            get
            {
                return instance.Value;
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
                if(e.Entity.Value == 1 ) { 
                NotificationService.AddNotification(userProfileTarget.Id, userProfileActor.Name + " liked your post",
                    "", userProfileActor.Name + " liked your post. Click to see your post", "posts/" + post.Id);
                }
                else
                {
                    NotificationService.AddNotification(userProfileTarget.Id, userProfileActor.Name + " disliked your post",
                        "", userProfileActor.Name + " disliked your post. Click to see your post", "posts/" + post.Id);
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
                    "", userProfileActor.Name + " commented your post. Click to see the post", "posts/" + parentPost.Id);
                
            }
        }

        private void BroadcastLikePrice(Like Like)
        {
            Clients.All.updateLikePrice(Like);
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
                }

                disposedValue = true;
            }
        }

        ~LikeWatcher()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}