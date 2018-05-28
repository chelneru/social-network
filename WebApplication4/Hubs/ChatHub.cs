using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WebApplication4.SignalIR;

namespace WebApplication4.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        #region---Data Members---
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();
        #endregion

        #region---Methods---
        public void Connect(string userName, Guid UserId)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName, UserId = UserId });
            }
            UserDetail CurrentUser = ConnectedUsers.Where(u => u.ConnectionId == id).FirstOrDefault();
            // send to caller           
            Clients.Caller.onConnected(CurrentUser.UserId.ToString(), CurrentUser.UserName, ConnectedUsers, CurrentMessage, CurrentUser.UserId);
            // send to all except caller client           
            Clients.AllExcept(CurrentUser.ConnectionId).onNewUserConnected(CurrentUser.UserId.ToString(), CurrentUser.UserName, CurrentUser.UserId);
        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            //AddMessageinCache(userName, message);

            // Broad cast message
            //Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            try
            {
                string fromconnectionid = Context.ConnectionId;
                string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserId).FirstOrDefault()).ToString();
                Guid _fromUserId = new Guid();
                Guid.TryParse(strfromUserId, out _fromUserId);
                Guid _toUserId = new Guid();
                Guid.TryParse(toUserId, out _toUserId);
                List<UserDetail> FromUsers = ConnectedUsers.Where(u => u.UserId == _fromUserId).ToList();
                List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserId == _toUserId).ToList();

                if (FromUsers.Count != 0 && ToUsers.Count() != 0)
                {
                    foreach (var ToUser in ToUsers)
                    {
                        // send to                                                                                            //Chat Title
                        Clients.Client(ToUser.ConnectionId).sendPrivateMessage(_fromUserId.ToString(), FromUsers[0].UserName, FromUsers[0].UserName, message);
                    }


                    foreach (var FromUser in FromUsers)
                    {
                        // send to caller user                                                                                //Chat Title
                        Clients.Client(FromUser.ConnectionId).sendPrivateMessage(_toUserId.ToString(), FromUsers[0].UserName, ToUsers[0].UserName, message);
                    }
                    // send to caller user
                    //Clients.Caller.sendPrivateMessage(_toUserId.ToString(), FromUsers[0].userName, message);
                    //ChatDB.Instance.SaveChatHistory(_fromUserId, _toUserId, message);
                    MessageDetail _MessageDeail = new MessageDetail { FromUserId = _fromUserId, FromUserName = FromUsers[0].UserName, ToUserId = _toUserId, ToUserName = ToUsers[0].UserName, Message = message };
                    AddMessageinCache(_MessageDeail);
                }
            }
            catch { }
        }

        public void RequestLastMessage(Guid FromUserId, Guid ToUserId)
        {
            List<MessageDetail> CurrentChatMessages = (from u in CurrentMessage where ((u.FromUserId == FromUserId && u.ToUserId == ToUserId) || (u.FromUserId == ToUserId && u.ToUserId == FromUserId)) select u).ToList();
            //send to caller user
            Clients.Caller.GetLastMessages(ToUserId, CurrentChatMessages);
        }

        public void SendUserTypingRequest(string toUserId)
        {
            string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserId).FirstOrDefault()).ToString();

            Guid _toUserId = new Guid();
            Guid.TryParse(toUserId, out _toUserId);
            List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserId == _toUserId).ToList();

            foreach (var ToUser in ToUsers)
            {
                // send to                                                                                            
                Clients.Client(ToUser.ConnectionId).ReceiveTypingRequest(strfromUserId);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                if (ConnectedUsers.Where(u => u.UserId == item.UserId).Count() == 0)
                {
                    var id = item.UserId.ToString();
                    Clients.All.onUserDisconnected(id, item.UserName);
                }
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region---private Messages---
        private void AddMessageinCache(MessageDetail _MessageDetail)
        {
            CurrentMessage.Add(_MessageDetail);
            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }
        #endregion
    }
}