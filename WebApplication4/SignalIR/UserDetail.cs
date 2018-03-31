using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.SignalIR
{
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public Guid UserID { get; set; }
        public string UserName { get; set; }
    }
}