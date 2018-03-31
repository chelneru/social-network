using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.SignalIR
{
    public class NotificationUser
    {
        public string ProfileId
        {
            get;
            set;
        }
        public HashSet<string> ConnectionIds
        {
            get;
            set;
        }
    }
}