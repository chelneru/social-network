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
    [RequireHttps]
    public class HomeController : Controller
    {
        private readonly NotificationService _notificationService = new NotificationService();
        private readonly UserProfileService _userProfileService = new UserProfileService();
        private readonly FriendsService _friendsService = new FriendsService();
        private readonly UserService _userService = new UserService();
        private readonly PostService _postService = new PostService();
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationDbContext ApplicationDbContext { get; set; }


        //protected ApplicationDbContext ApplicationDbContext { get; set; }

        //protected UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public ActionResult Index()
        {

            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                //an user is logged in

                var userProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));
                if(userProfile == null)
                {
                    //new user so we create new UserProfile
                    var user = _userService.GetUserById(userId);
                    var newUserProfile = _userProfileService.CreateNewUserProfile(user);
                    _friendsService.CreateEmptyFriendsEntity(newUserProfile);

                    userProfile = newUserProfile;
                }
                List<HomeIndexPostViewModel> posts = null;
                if (userProfile != null) { 
                posts = _postService.GetPosts(userProfile.Id);
                }
                
                var viewModel = new HomeIndexViewModel
                {
                    Posts = posts,
                    PostModel = new Post()
                };
                return View(viewModel);
            }
            else
            {
                // no user is logged in
                ViewBag.userAddress = '0';
                return View(new HomeIndexViewModel());
            }

           
            
        }
        [HttpPost, ActionName("search")]
        [Route("search/", Name = "search")]
        [ValidateAntiForgeryToken]

        public JsonResult Search()
        {
            var textToSearch = Request.Form["search_query"];
            var userProfiles = _userProfileService.SearchUserProfilesByUserName(textToSearch);
            var posts = _postService.SearchPostsByContent(textToSearch);
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