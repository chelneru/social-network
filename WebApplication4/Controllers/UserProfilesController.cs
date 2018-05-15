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
        private readonly UserProfileService  _userProfileService= new UserProfileService();
        private readonly FriendRequestService  _friendRequestService= new FriendRequestService();
        private readonly FriendsService  _friendsService= new FriendsService();
        private readonly PostService  _postService= new PostService();
        private readonly NotificationService  _notificationService = new NotificationService();
        
        // GET: UserProfiles
        public ActionResult Index()
        {
            var userProfiles = _userProfileService.GetAllUserProfiles();
            foreach (var up in userProfiles)
            {
                var relativePath = "~/Content/avatars/" + up.Id + "_avatar.jpg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (System.IO.File.Exists(absolutePath))
                {
                    up.AvatarUrl = "../../Content/avatars/" + up.Id + "_avatar.jpg";
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

            var relativePath = "~/Content/avatars/" + userProfile.Id + "_avatar.jpg";
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                userProfile.AvatarUrl = "../../Content/avatars/" + userProfile.Id + "_avatar.jpg";
            }
            else
            {
                userProfile.AvatarUrl = "../../Content/avatars/default_avatar.jpeg";
            }

            var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(User.Identity.GetUserId()));

            var friendRequest = _friendRequestService.CheckIfFriendRequestExists(currentUserProfile.Id, userProfile.Id);
            
            var viewModel = new UserProfileDetailsViewModel
            {
                Profile = userProfile,
                FriendsCollection = _userProfileService.GetUserProfileFriends(userProfile.Id),
                ActiveFriendRequest = friendRequest
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
            var viewModel = new GalleryViewModel
            {
                Posts = list,
                UserProfile = userProfile
            };
            return View(viewModel);
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

        [HttpPost, ActionName("accept-deny-friend-request")]
        [Route("friend-request", Name = "accept-deny-friend-request")]
        [ValidateAntiForgeryToken]
        public JsonResult RespondToFriendRequest()
        {    
            if (Request.Form["response"] != null && Request.Form["initiatorProfileId"] != null)
            {
                var currentUserId = User.Identity.GetUserId();
                var currentProfile = _userProfileService.GetUserProfileByUserId(new Guid(currentUserId));
                var friendRequest =
                    _friendRequestService.CheckIfFriendRequestExists(new Guid(Request.Form["initiatorProfileId"]), currentProfile.Id);
                if (friendRequest != null)
                {
                    if (!short.TryParse(Request.Form["response"], out var response))
                    {
                        response = 0;
                    }

                    if (response == 1 || response == 2)
                    {
                        //we have a valid response
                        _friendRequestService.MarkFriendRequestAsUsed(friendRequest.Id, response);
                        if (Request.Form["notificationId"] != null)
                        {
                           // in case this request comes from a notification
                            _notificationService.MarkNotificationAsSeen(Request.Form["notificationId"]);

                        }
                        
                        if (response == 1)
                        {
                            //friend request accepted , add user profile as friend
                            _friendsService.AddFriend(currentProfile.Id, new Guid(Request.Form["initiatorProfileId"]));
                            return Json(new {Message = "friend added"});
                        }
                        else
                        {
                            return Json(new {Message = "friend request denied"});   
                        }
                    }
                    else
                    {
                        return Json(new {Message = "invalid response"});
                    }
                }
                else
                {
                    return Json(new {Message = "no friend request found"});
                }
            }
            else
            {
                return Json(new {Message = "missing response"});
            }
        }

        [HttpPost, ActionName("add-remove-friend")]
        [Route("add-remove-friend/", Name = "add-remove-friend")]
        [ValidateAntiForgeryToken]
        public JsonResult AddFriend()
        {
            if (Request.Form["user_id"] != null)
            {
                var targetUserProfile = _userProfileService.GetUserProfile(new Guid(Request.Form["user_id"]));
                var initiatorUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(User.Identity.GetUserId()));

                var friendRequest = _friendRequestService.CheckIfFriendRequestExists(initiatorUserProfile.Id, targetUserProfile.Id);
                if (friendRequest != null)
                {
                    //a friend request is active so we cancel it
                    _friendRequestService.DeleteFriendRequest(friendRequest.Id);
                    return Json(new { Message = "friend request canceled" });
                }
                
                var friendship = _friendsService.CheckFriendship(targetUserProfile.Id, initiatorUserProfile.Id);
                
                if (friendship == true)
                {
                    //user is already friend so we unfriend him
                    _friendsService.RemoveFriend(targetUserProfile.Id, initiatorUserProfile.Id);
                    return Json(new { Message = "friend removed" });
                }
                else
                {
                    // user is not friend so we send friend request
                    var friendRequestFeedback = _friendRequestService.CreateFriendRequest(initiatorUserProfile.Id, targetUserProfile.Id);
                    if (friendRequestFeedback == true)
                    {
                        return Json(new { Message = "friend request sent" });
                    }
                    else
                    {
                        return Json(new { Message = "failed to send friend request" });
                    }
                }
            }
            return Json(new {Message = "invalid user_id parameter"});
        }

        [Route("api/upload-avatar/", Name = "upload-avatar")]
        [ValidateAntiForgeryToken]
        public JsonResult UploadAvatar()
        {
            var response = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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
                        var userProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));

                        var path = Path.Combine(Server.MapPath("~/Content/Avatars"), userProfile.Id + "_avatar.jpg");
                        img.Save(path); 
                        var dbPath = "../../Content/Avatars/" + userId + "_avatar.jpg";
                        _userProfileService.ChangeUserProfileAvatar(userProfile.Id, dbPath);
                    }
                    catch (Exception e)
                    {
                        response.Data = new {message = e.Message};
                    }
                }
            }
            catch (Exception e)
            {
//                Response.StatusCode = (int) HttpStatusCode.BadRequest;
//                response.Data = new {message = e.Message + " " + e.StackTrace};
//                return response;
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
