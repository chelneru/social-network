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

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();


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
               
                var userProfile = unitOfWork.UserProfileRepository.Get(includeProperties: "User").First(x => x.User.Id == user);
 
                System.Web.HttpContext.Current.Session["userAddress"] = userProfile.UserAddress;
                System.Web.HttpContext.Current.Session["userName"] = userProfile.Name;
                System.Web.HttpContext.Current.Session["userId"] = userProfile.User.Id;

            }
            else
            {
                // no user is logged in
                ViewBag.userAddress = '0';
            }

            var posts = unitOfWork.PostRepository.Get(includeProperties: "UserProfile,Likes").Select(p => new HomeIndexPostViewModel()
            {
                Id = p.Id,
                Edited = p.Edited,
                PostDateTime = p.PostDateTime,
                Content = p.Content,
                ParentPost = p.ParentPost,
                UserAddress = p.UserProfile.UserAddress,
                UserName = p.UserProfile.Name,
                PhotoLink = p.PhotoLink,
                VideoLink = p.VideoLink,
                ShareLink = p.ShareLink,
                Likes = p.Likes.Sum(l => l.Value),
                CurrentUserVote = (Int16)p.Likes.Where(l => l.UserProfile.Id == p.UserProfile.Id).Select(l => l.Value).FirstOrDefault()
            }).ToList();

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
            var userProfiles = unitOfWork.UserProfileRepository.Get(includeProperties: "User").Where(x => x.User.UserName.Contains(textToSearch)).Select(x => new { x.UserAddress, x.User.UserName }).ToList();
            var posts = unitOfWork.PostRepository.Get().Where(x => x.Content.Contains(textToSearch)).Select(x => new { x.Id, x.Content}).ToList();
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
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}