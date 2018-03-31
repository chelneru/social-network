using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.SignalIR
{
    public class MessageDetail
    {
        public Guid FromUserID { get; set; }
        public string FromUserName { get; set; }
        public Guid ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Message { get; set; }
    }

}