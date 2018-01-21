using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication4.Models;
using WebApplication4.DAL;
using WebApplication4.ViewModels;

namespace WebApplication4.Controllers
{
    public class PostController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        //private ApplicationDbContext ApplicationDbContext { get; set; }
        //private UserManager<ApplicationUser> UserManager { get; set; }

        //public PostController()
        //{
        //    ApplicationDbContext = new ApplicationDbContext();
        //    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
        //}
        public ActionResult Index()
        {
            var list = unitOfWork.PostRepository.Get();
            return View(list);
        }
        //
        //        // GET: Post/Details/5
        //public ActionResult Details(int id)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Model.Content")]HomeIndexViewModel viewModel)
        {

            var post = new Post
            {
                UserId = User.Identity.GetUserId(),
                User = unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()),
                Content = Request.Form.Get("Model.Content"),
                PostDateTime = DateTime.Now,
                Id = Guid.NewGuid()
            };


            try
            {
                if (ModelState.IsValid)
                {

                    unitOfWork.PostRepository.Insert(post);
                    unitOfWork.Save();
                }
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
