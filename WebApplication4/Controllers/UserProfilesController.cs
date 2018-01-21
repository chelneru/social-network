using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using WebApplication4.DAL;

namespace WebApplication4.Controllers
{
    public class UserProfilesController : Controller
    {

        //private ApplicationDbContext db = new ApplicationDbContext();
        //protected UserManager<ApplicationUser> UserManager { get; set; }
        //public UserProfilesController()
        //{
        //    this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));

        //}
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: UserProfiles
        public ActionResult Index()
        {
            return View(unitOfWork.UserProfileRepository.Get());
        }
        [Route("users/{userAddress}/", Name = "users")]
        // GET: UserProfiles/users/{userAddress}/
        public ActionResult Details(string userAddress)
        {
            if (userAddress == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userProfile = unitOfWork.UserProfileRepository.Get(includeProperties: "User").First(x => x.UserAddress == userAddress);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            else
            {

                var currentUserId = User.Identity.GetUserId();
                var relativePath = "~/Content/avatars/" + userProfile.User.Id + "_avatar.jpeg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (System.IO.File.Exists(absolutePath))
                {
                    userProfile.AvatarUrl = "../../Content/avatars/" + userProfile.User.Id + "_avatar.jpeg"; ;
                }
                else
                {
                    userProfile.AvatarUrl = "../../Content/avatars/default_avatar.jpeg";
                }

                return View(userProfile);
            }
        }

        // GET: UserProfiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,JoinDate,Location,AvatarUrl,About,BirthDate")] UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                return View(userProfile);

            }
            userProfile.Id = Guid.NewGuid();
            unitOfWork.UserProfileRepository.Insert(userProfile);
            unitOfWork.Save();
            return RedirectToAction("Index");

        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = unitOfWork.UserProfileRepository.GetByID(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,JoinDate,Location,About,BirthDate,userAddress")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.UserProfileRepository.Update(userProfile);

                unitOfWork.Save();
                var current_user_id = User.Identity.GetUserId();
                userProfile.User = unitOfWork.UserRepository.Get().First(x => x.Id == current_user_id.ToString());
                System.Web.HttpContext.Current.Session["userAddress"] = userProfile.UserAddress;

                return RedirectToAction("Index");
            }
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = unitOfWork.UserProfileRepository.GetByID(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        [Route("users/{userAddress}/gallery", Name = "gallery")]
        [HttpGet, ActionName("Gallery")]
        public ActionResult Gallery(string userAddress)
        {
            var userProfile = unitOfWork.UserProfileRepository.Get(includeProperties: "User").First(x => x.UserAddress == userAddress);

            if (userProfile == null)
            {
                return HttpNotFound();
            }
            var photos = Enumerable.Empty<string>();
            try
            {
                photos = Directory.EnumerateFiles(Server.MapPath("~/Content/galleries/" + userProfile.User.Id.ToString()));
            }
            catch (Exception e)
            {

            }
            var list = photos.ToList();
            for (var i = 0; i < list.Count(); i++)
            {
                list[i] = list[i].Replace("E:\\Visual Studio Projects\\social-network\\WebApplication4", "../../");
            }

            return View(list);
        }
        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var userProfile = unitOfWork.UserProfileRepository.GetByID(id);
            unitOfWork.UserProfileRepository.Delete(userProfile);

            unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpPost, ActionName("upload-photos")]
        [Route("api/upload-photos/", Name = "upload-photos")]
        [ValidateAntiForgeryToken]

        public ActionResult SaveUploadedFile()
        {
            var currentUserId = User.Identity.GetUserId();
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\galleries", Server.MapPath(@"\")));
                        var userId = User.Identity.GetUserId();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), userId.ToString());

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);

                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }
        [Route("api/upload-avatar/", Name = "upload-avatar")]
        [ValidateAntiForgeryToken]

        public JsonResult UploadAvatar()
        {
            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {

                var fileContent = Request.Files["avatar"];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk
                    Bitmap img = new Bitmap(stream);

                    if (img.Height > Classes.Constants.MAXHEIGHT || img.Width > Classes.Constants.MAXWIDTH)
                    {
                        img = new Bitmap(img, new Size(img.Height - (img.Height - 500), img.Width - (img.Width - 500)));
                    }
                    try
                    {
                        var userId = User.Identity.GetUserId();
                        var path = Path.Combine(Server.MapPath("~/Content/Avatars"),
                            userId.ToString() + "_avatar.jpeg");
                        img.Save(path);
                    }
                    catch (Exception e)
                    {
                        response.Data = new { message = "No user found." }
                        ;
                    }

                }
            }
            catch (Exception e)
            {

                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                response.Data = new { message = e.Message + " " + e.StackTrace };
                return response;

            }
            response.Data = new { message = "File uploaded successfully" };
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
