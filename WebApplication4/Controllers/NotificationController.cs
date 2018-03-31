using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.SignalIR;

namespace WebApplication4.Controllers
{
    public class NotificationController : Controller
    {
        public JsonResult GetNotifications()
        {
            var message = "23131";
            return new JsonResult { Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
