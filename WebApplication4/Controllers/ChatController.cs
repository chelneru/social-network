using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Controllers
{
    public class ChatController : Controller
    {
        [HttpGet]
        [Route("chat/", Name = "chat")]
        public ActionResult Index()
        {
            return View();
        }
    }
}