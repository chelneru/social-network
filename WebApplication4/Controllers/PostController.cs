using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext ApplicationDbContext { get; set; }
        private UserManager<ApplicationUser> UserManager { get; set; }

        public PostController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
        }
        //        // GET: Post
        //        public ActionResult Index()
        //        {
        //            return View();
        //        }
        //
        //        // GET: Post/Details/5
        //        public ActionResult Details(int id)
        //        {
        //            return View();
        //        }
        //
        //        // GET: Post/Create
        //        public ActionResult Create()
        //        {
        //            return View();
        //        }

        // POST: Post/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Content")]Post model)
        {

            model.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            model.PostDateTime = DateTime.Now;
            try
            {

                var user = UserManager.FindById(User.Identity.GetUserId());
                var newPost = new Post
                {

                    Content = model.Content,
                    PostDateTime = DateTime.Now,
                    User = user

                };
                ApplicationDbContext.Post.Add(newPost);
                ApplicationDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            return RedirectToAction("Index", "Home");
        }

        //        // GET: Post/Edit/5
        //        public ActionResult Edit(int id)
        //        {
        //            return View();
        //        }
        //
        //        // POST: Post/Edit/5
        //        [HttpPost]
        //        public ActionResult Edit(int id, FormCollection collection)
        //        {
        //            try
        //            {
        //                // TODO: Add update logic here
        //
        //                return RedirectToAction("Index");
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }
        //
        //        // GET: Post/Delete/5
        //        public ActionResult Delete(int id)
        //        {
        //            return View();
        //        }
        //
        //        // POST: Post/Delete/5
        //        [HttpPost]
        //        public ActionResult Delete(int id, FormCollection collection)
        //        {
        //            try
        //            {
        //                // TODO: Add delete logic here
        //
        //                return RedirectToAction("Index");
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }
    }
}
