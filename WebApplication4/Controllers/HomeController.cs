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

            }
            else
            {
                // no user is logged in
                ViewBag.userAddress = '0';
            }

            var posts = unitOfWork.PostRepository.Get(includeProperties: "User,Likes").ToList();
            var viewModel = new HomeIndexViewModel
            {
                Posts = posts,
                postModel = new Post()
            };
            return View(viewModel);
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