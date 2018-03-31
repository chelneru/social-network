using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TableDependency;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using WebApplication4.Models;

namespace WebApplication4.SqlDependency
{
    public class SqlDependencyService
    {
        // Singleton instance
        private readonly static Lazy<SqlDependencyService> _instance = new Lazy<SqlDependencyService>(
            () => new SqlDependencyService(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients));

        private static SqlTableDependency<Like> _tableDependency;

        private SqlDependencyService(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            var mapper = new ModelToTableMapper<Like>();

            _tableDependency = new SqlTableDependency<Like>(
                ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString,
                "Likes",
                mapper);

            _tableDependency.OnChanged += SqlTableDependency_Changed;
            _tableDependency.OnError += SqlTableDependency_OnError;
            _tableDependency.Start();
        }

        public static SqlDependencyService Instance
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
            var LikeList = new List<Like>();

            var connectionString = ConfigurationManager.ConnectionStrings
                    ["connectionString"].ConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM [Likes]";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var userId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("UserId"));
                            var postId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PostId"));
                            var Value = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Value"));
                         
                            //LikeList.Add(new Like  { UserProfile = userId,  PostId = new Guid(postId), Value = Value});
                        }
                    }
                }
            }

            return LikeList;
        }

        void SqlTableDependency_OnError(object sender, ErrorEventArgs e)
        {
            throw e.Error;
        }

        /// <summary>
        /// Broadcast New Like Price
        /// </summary>
        void SqlTableDependency_Changed(object sender, RecordChangedEventArgs<Like> e)
        {
            if (e.ChangeType != TableDependency.Enums.ChangeType.None)
            {
                BroadcastLikePrice(e.Entity);
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
                    _tableDependency.Stop();
                }

                disposedValue = true;
            }
        }

        ~SqlDependencyService()
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
