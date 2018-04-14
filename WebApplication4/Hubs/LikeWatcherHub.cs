using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;
using WebApplication4.SignalIR;
using WebApplication4.SqlTableDependencyClasses;

namespace WebApplication4.Hubs
{
    [HubName("LikeWatcher")]
    public class LikeWatcherHub : Hub
    {
        private readonly LikeWatcher _likeWatcher;
        private static List<UserDetail> ConnectedUsers = new List<UserDetail>();

        public LikeWatcherHub() :
            this(LikeWatcher.Instance)
        {

        }

        public LikeWatcherHub(LikeWatcher likeWatcher)
        {
            _likeWatcher = likeWatcher;
        }

        public IEnumerable<Like> GetAllStocks()
        {
            return _likeWatcher.GetAllLikes();
        }
    }
}