using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication4.Models;
using WebApplication4.ViewModels;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public ActionResult Index()
        {

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                //an user is logged in
                var userProfile = ApplicationDbContext.UserProfile.First(x => x.ApplicationUser.Id == user);
                System.Web.HttpContext.Current.Session["userAddress"] = userProfile.UserAddress; 

            }
            else
            {
                // no user is logged in
                ViewBag.userAddress = '0';
            }
            var posts = this.ApplicationDbContext.Post.Include("User").OrderBy(x => x.PostDateTime).ToList();
            var viewParameter = new HomeIndexViewModel(posts);

            return View(viewParameter);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}