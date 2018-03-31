using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication4.Models;
using WebApplication4.DAL;
using WebApplication4.ViewModels;
using System.Linq;
using System.Data.Entity.Validation;

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
            var list = unitOfWork.PostRepository.Get(includeProperties: "User,Likes").Select(p => new Post()
            {
                Content = p.Content,
                ParentPost = p.ParentPost,
                UserProfile =p.UserProfile,
                Likes = p.Likes.Select(l => new Like() { Value = l.Value }).ToList()
            }).ToList();
            return View(list);
        }

        [Route("posts/{postId}/", Name = "posts")]
        public ActionResult Details(string postId)
        {
            var post = unitOfWork.PostRepository.Get(includeProperties: "UserProfile,Likes").First(x => x.Id == new Guid(postId));
            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                Votes = post.Likes.Sum(x => x.Value)
            };
            return View(viewModel);
        }
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
            Post parentPost = null;
            if (Request.Form.Get("postModel.parentPost") != null)
            {
                parentPost = unitOfWork.PostRepository.Get().First(x => x.Id == new Guid(Request.Form.Get("postModel.ParentPost")));
            }

            var userId = User.Identity.GetUserId();
            var current_user_profile = unitOfWork.UserProfileRepository.Get(includeProperties: "User").FirstOrDefault(up => up.User.Id == userId);
            var post = new Post
            {
                UserProfileId = current_user_profile.Id,
                UserProfile = current_user_profile,
                Content = Request.Form.Get("Model.Content") == null ? Request.Form.Get("postModel.Content") : Request.Form.Get("Model.Content"),
                PostDateTime = DateTime.Now,
                ParentPost = parentPost,
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
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("vote-post")]
        [Route("vote-post/", Name = "vote-post")]

        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult VotePost()
        {
            if (Request.Form["post_id"] != null)

            {
                var value = 0;

                if (Int32.TryParse(Request.Form["value"], out value))
                {
                    value = value > 0 ? 1 : -1;
                    var post_id = new Guid(Request.Form["post_id"]);

                    var current_user_profile = unitOfWork.UserProfileRepository.Get(includeProperties: "User").FirstOrDefault(x => x.User.Id == User.Identity.GetUserId());
                    var post = unitOfWork.PostRepository.Get().FirstOrDefault(x => x.Id == post_id);
                    var existing_like = unitOfWork.LikeRepository.Get().FirstOrDefault(x => x.Post.Id == post_id && x.UserProfileId == current_user_profile.Id);
                    if (existing_like == null)
                    {
                        var vote = new Like()
                        {
                            Id = Guid.NewGuid(),
                            UserProfile = current_user_profile,
                            UserProfileId = current_user_profile.Id,
                            Value = value,
                            Post = post,
                            PostId = post.Id
                        };
                        unitOfWork.LikeRepository.Insert(vote);

                        unitOfWork.Save();
                        return Json(new { Message = "vote_registered" });
                    }
                    else
                    {
                        if (existing_like.Value != value)
                        {
                            existing_like.Value = value;
                            unitOfWork.Save();
                            return Json(new { Message = "vote_registered" });
                        }
                        else
                        {
                            return Json(new { Message = "already_voted" });

                        }
                    }

                }
                else
                {
                    return Json(new { Message = "invalid_parameter" });
                }
            }
            else
            {
                return Json(new { Message = "invalid_parameter" });

            }

        }
    }
}
