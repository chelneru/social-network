using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using WebApplication4.Services;
using System.Collections.ObjectModel;
using WebApplication4.ViewModels;
using System.Data.Entity.Validation;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using WebApplication4.DAL;

namespace WebApplication4.Controllers
{
    public class UserProfilesController : Controller
    {

        private readonly PostService _postService;
        private readonly UserProfileService _userProfileService;
        private readonly LikeService _likeService;
        private readonly FriendsService _friendsService;
        
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public UserProfilesController(PostService postService, UserProfileService userProfileService, LikeService likeService)
        {
            _postService = postService;
            _userProfileService = userProfileService;
            _likeService = likeService;
        }
        // GET: UserProfiles
        public ActionResult Index()
        {
            var userProfiles = _userProfileService.GetAllUserProfiles();
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

            var userProfile = _userProfileService.GetUserProfileByUserAddress(userAddress);
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
                FriendsCollection = _userProfileService.GetUserProfileFriends(userProfile.Id)
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
            _userProfileService.InsertUserProfile(userProfile);
            
            return RedirectToAction("Index");
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userProfile = _userProfileService.GetUserProfile(id.Value);
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
                _userProfileService.UpdateUserProfile(userProfile);
                var currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                userProfile.User = currentUser;
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

            UserProfile userProfile = _userProfileService.GetUserProfile(id.Value);
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
            var userProfile = _userProfileService.GetUserProfileByUserAddress(userAddress);
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            var list = _postService.GetUserPhotos(userProfile.Id);
            return View(list);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _userProfileService.DeleteUserProfile(id);
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
                            new DirectoryInfo(string.Format($"{0}Content\\galleries", Server.MapPath(@"\")));
                        var userId = User.Identity.GetUserId();
                        var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));
                        if (currentUserProfile != null)
                        {
                            var pathString = Path.Combine(originalDirectory.ToString(), userId);
                            var isExists = Directory.Exists(pathString);
                            if (!isExists) Directory.CreateDirectory(pathString);
                            var path = string.Format($"{0}\\{1}", pathString, file.FileName);
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
                                _postService.AddPost(currentUserProfile, "descriere", null, null, path.Replace(
                                    "E:\\Visual Studio Projects\\social-network\\WebApplication4",
                                    "../../"));
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
                var userProfile = _userProfileService.GetUserProfile(new Guid(Request.Form["user_id"]));
                    
                   
                var friendsCollection = _userProfileService.GetUserProfileFriends(userProfile.Id);
                var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(User.Identity.GetUserId()));
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
                        _friendsService.AddFriends(friendsCollection);
                        return Json(new {Message = "friend added"});
                    }

                    var findCurrentUserAsFriend =
                        friendsCollection.Friend_UserProfile.FirstOrDefault(x => x.Id == currentUserProfile.Id);
                    if (findCurrentUserAsFriend != null)
                    {
                        //user is already friend so we unfriend him
                        friendsCollection.Friend_UserProfile.Remove(currentUserProfile);
                        _friendsService.RemoveFriend(friendsCollection.UserProfile.Id,currentUserProfile.Id);
                        return Json(new {Message = "friend removed"});
                    }

                    friendsCollection.Friend_UserProfile.Add(currentUserProfile);
                    _friendsService.AddFriend(friendsCollection.UserProfile.Id,currentUserProfile.Id);
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
            var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));
            var post = _postService.GetPost(new Guid(postId));
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
                Comments = _postService.GetComments(new Guid(postId))
            };
            return View(viewModel);
        }

        
    }
}
