using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using WebApplication4.DAL;
using System.Collections.ObjectModel;
using WebApplication4.ViewModels;
using System.Data.Entity.Validation;

namespace WebApplication4.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: UserProfiles
        public ActionResult Index()
        {
            var userProfiles = _unitOfWork.UserProfileRepository.Get(includeProperties: "User");
            foreach (var up in userProfiles)
            {
                var relativePath = "~/Content/avatars/" + up.User.Id + "_avatar.jpg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (System.IO.File.Exists(absolutePath))
                {
                    up.AvatarUrl = "../../Content/avatars/" + up.User.Id + "_avatar.jpg";
                }
                else
                {
                    up.AvatarUrl = "../../Content/avatars/default_avatar.jpeg";
                }
            }

            return View(userProfiles);
        }

        [Route("users/{userAddress}/", Name = "users")]
        // GET: UserProfiles/users/{userAddress}/
        public ActionResult Details(string userAddress)
        {
            if (userAddress == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userProfile = _unitOfWork.UserProfileRepository.Get(includeProperties: "User")
                .First(x => x.UserAddress == userAddress);
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            var relativePath = "~/Content/avatars/" + userProfile.User.Id + "_avatar.jpg";
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                userProfile.AvatarUrl = "../../Content/avatars/" + userProfile.User.Id + "_avatar.jpg";
            }
            else
            {
                userProfile.AvatarUrl = "../../Content/avatars/default_avatar.jpeg";
            }
            //var friendsCollection = _unitOfWork.FriendsRepository.Get().First(x => x.UserProfile.Id == userProfile.Id);
            //friendsCollection.Friend_UserProfile.Add(_unitOfWork.UserProfileRepository.Get().First(x => x.Name =="ascensionter"));
            //_unitOfWork.Save();

            var viewModel = new UserProfileDetailsViewModel
            {
                Profile = userProfile,
                FriendsCollection = _unitOfWork.FriendsRepository.Get()
                    .FirstOrDefault(x => x.UserProfile.Id == userProfile.Id)
            };
            if (viewModel.FriendsCollection == null)
            {
                viewModel.FriendsCollection = new Friends
                {
                    Friend_UserProfile = new Collection<UserProfile>(),
                    UserProfile = new UserProfile()
                };
            }

            return View(viewModel);
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
        public ActionResult Create([Bind(Include = "Id,Name,JoinDate,Location,AvatarUrl,About,BirthDate")]
            UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                return View(userProfile);
            }

            userProfile.Id = Guid.NewGuid();
            _unitOfWork.UserProfileRepository.Insert(userProfile);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserProfile userProfile = _unitOfWork.UserProfileRepository.GetByID(id);
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
        public ActionResult Edit([Bind(Include = "Id,Name,JoinDate,Location,About,BirthDate,userAddress")]
            UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.UserProfileRepository.Update(userProfile);
                _unitOfWork.Save();
                var currentUserId = User.Identity.GetUserId();
                userProfile.User = _unitOfWork.UserRepository.Get().First(x => x.Id == currentUserId.ToString());
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

            UserProfile userProfile = _unitOfWork.UserProfileRepository.GetByID(id);
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
            var userProfile = _unitOfWork.UserProfileRepository.Get(includeProperties: "User")
                .First(x => x.UserAddress == userAddress);
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            var photos = _unitOfWork.PostRepository.Get(x => x.UserProfileId == userProfile.Id && x.PhotoLink != null);
            var list = photos.ToList();
            return View(list);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var userProfile = _unitOfWork.UserProfileRepository.GetByID(id);
            _unitOfWork.UserProfileRepository.Delete(userProfile);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("upload-photos")]
        [Route("api/upload-photos/", Name = "upload-photos")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUploadedFile()
        {
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        fName = file.FileName;
                        var originalDirectory =
                            new DirectoryInfo(string.Format("{0}Content\\galleries", Server.MapPath(@"\")));
                        var userId = User.Identity.GetUserId();
                        var currentUserProfile = _unitOfWork.UserProfileRepository.Get(includeProperties: "User")
                            .FirstOrDefault(up => up.User.Id == userId);
                        if (currentUserProfile != null)
                        {
                            string pathString = Path.Combine(originalDirectory.ToString(), userId);
                            bool isExists = Directory.Exists(pathString);
                            if (!isExists) Directory.CreateDirectory(pathString);
                            var path = string.Format("{0}\\{1}", pathString, file.FileName);
                            file.SaveAs(path);
                            var post = new Post
                            {
                                Content = "descriere", //TODO add content to photo
                                UserProfile = currentUserProfile,
                                UserProfileId = currentUserProfile.Id,
                                PostDateTime = DateTime.Now,
                                Id = Guid.NewGuid(),
                                PhotoLink = path.Replace("E:\\Visual Studio Projects\\social-network\\WebApplication4",
                                    "../../")
                            };
                            try
                            {
                                _unitOfWork.PostRepository.Insert(post);
                                _unitOfWork.Save();
                            }
                            catch (DbEntityValidationException e)
                            {
                                foreach (var exception in e.EntityValidationErrors)
                                {
                                    System.Diagnostics.Debug.WriteLine(exception.ValidationErrors.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new {ex.Message});
            }

            return Json(new {Message = fName});
        }

        [HttpPost, ActionName("add-remove-friend")]
        [Route("add-remove-friend/", Name = "add-remove-friend")]
        [ValidateAntiForgeryToken]
        public JsonResult AddFriend()
        {
            if (Request.Form["user_id"] != null)
            {
                var userProfile = _unitOfWork.UserProfileRepository.Get()
                    .First(x => x.Id.ToString() == Request.Form["user_id"]);
                var friendsCollection = _unitOfWork.FriendsRepository.Get()
                    .FirstOrDefault(x => x.UserProfile.Id == userProfile.Id);
                var currentUserProfile = _unitOfWork.UserProfileRepository.Get(includeProperties: "User")
                    .FirstOrDefault(x => x.User.Id == User.Identity.GetUserId());
                if (currentUserProfile != null)
                {
                    if (friendsCollection == null)
                    {
                        // user doesn't have any friends so we add the current user as friend
                        friendsCollection = new Friends
                        {
                            UserProfile = userProfile,
                            Friend_UserProfile = new Collection<UserProfile>()
                        };
                        friendsCollection.Friend_UserProfile = new Collection<UserProfile> {currentUserProfile};
                        _unitOfWork.FriendsRepository.Insert(friendsCollection);
                        _unitOfWork.Save();
                        return Json(new {Message = "friend added"});
                    }

                    var findCurrentUserAsFriend =
                        friendsCollection.Friend_UserProfile.FirstOrDefault(x => x.Id == currentUserProfile.Id);
                    if (findCurrentUserAsFriend != null)
                    {
                        //user is already friend so we unfriend him
                        friendsCollection.Friend_UserProfile.Remove(currentUserProfile);
                        _unitOfWork.Save();
                        return Json(new {Message = "friend removed"});
                    }

                    friendsCollection.Friend_UserProfile.Add(currentUserProfile);
                    _unitOfWork.Save();
                    return Json(new {Message = "friend added"});
                }
            }

            return Json(new {Message = "invalid user_id parameter"});
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
                    var img = new Bitmap(stream);
                    if (img.Height > Classes.Constants.MAXHEIGHT || img.Width > Classes.Constants.MAXWIDTH)
                    {
                        img = new Bitmap(img, new Size(img.Height - (img.Height - 500), img.Width - (img.Width - 500)));
                    }

                    try
                    {
                        var userId = User.Identity.GetUserId();
                        var path = Path.Combine(Server.MapPath("~/Content/Avatars"), userId + "_avatar.jpeg");
                        img.Save(path);
                    }
                    catch (Exception e)
                    {
                        response.Data = new {message = e.Message};
                    }
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                response.Data = new {message = e.Message + " " + e.StackTrace};
                return response;
            }

            response.Data = new {message = "File uploaded successfully"};
            return response;
        }

        [Route("photos/{postId}/", Name = "photos")]
        public ActionResult PhotoDetails(string postId)
        {
            var userId = User.Identity.GetUserId();
            var currentUserProfile = _unitOfWork.UserProfileRepository.Get(includeProperties: "User")
                .FirstOrDefault(up => up.User.Id == userId);
            var post = _unitOfWork.PostRepository.Get(includeProperties: "UserProfile")
                .First(x => x.Id == new Guid(postId));
            var viewModel = new PhotoDetailsViewModel
            {
                Post = post,
                Votes = post.Likes.Sum(x => x.Value),
                CurrentUserVote =
                    currentUserProfile != null
                        ? post.Likes.Where(l => l.UserProfile.Id == currentUserProfile.Id)
                            .Select(l => l.Value)
                            .FirstOrDefault()
                        : 0,
                Comments = _unitOfWork.PostRepository.Get(includeProperties: "UserProfile")
                    .Where(x => x.ParentPost.Id == post.Id)
                    .ToList()
            };
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
