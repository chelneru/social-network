using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication4.Models;
using WebApplication4.ViewModels;
using AutoMapper;
using WebApplication4.DAL;
using System.Data.Entity.Core.Objects;
using WebApplication4.DAL.Interfaces;
using System.Collections.ObjectModel;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        private readonly NotificationService _notificationService = new NotificationService();
        private readonly UserProfileService _userProfileService = new UserProfileService();
        private readonly PostService _postService = new PostService();


        //protected ApplicationDbContext ApplicationDbContext { get; set; }

        //protected UserManager<ApplicationUser> UserManager { get; set; }

        //public HomeController()
        //{
        //    this.ApplicationDbContext = new ApplicationDbContext();
        //    this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        //}
        public ActionResult Index()
        {

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                //an user is logged in

                var userProfile = UserProfileService.GetUserProfileByUserId(new Guid(user));
                var notifications = NotificationService.GetNewNotifications(userProfile.Id);
                System.Web.HttpContext.Current.Session["notifications"] = notifications;
                System.Web.HttpContext.Current.Session["userAddress"] = userProfile.UserAddress;
                System.Web.HttpContext.Current.Session["userName"] = userProfile.Name;
                System.Web.HttpContext.Current.Session["userId"] = userProfile.User.Id;
                System.Web.HttpContext.Current.Session["userProfileId"] = userProfile.Id;

            }
            else
            {
                // no user is logged in
                ViewBag.userAddress = '0';
            }

            var posts = PostService.GetPosts();

            var viewModel = new HomeIndexViewModel
            {
                Posts = posts,
                postModel = new Post()
            };
            return View(viewModel);
        }
        [HttpPost, ActionName("search")]
        [Route("search/", Name = "search")]
        [ValidateAntiForgeryToken]

        public JsonResult Search()
        {
            var textToSearch = Request.Form["search_query"];
            var userProfiles = UserProfileService.SearchUserProfilesByUserName(textToSearch);
            var posts = PostService.SearchPostsByContent(textToSearch);
            var result = new
            {
                UsersProfiles = userProfiles,
                Posts = posts
            };
            return Json(result);
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