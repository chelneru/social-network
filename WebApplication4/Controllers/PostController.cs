using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using WebApplication4.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.DAL;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService = new PostService();
        private readonly UserProfileService _userProfileService = new UserProfileService(ApplicationDbContext.Create());
        private readonly LikeService _likeService = new LikeService();
        private readonly LinkPreviewService _linkPreviewService = new LinkPreviewService();

        public ActionResult Index()
        {
            var list = _postService.GetPosts();
            return View(list);
        }

        [Route("posts/{postId}/", Name = "posts")]
        public ActionResult Details(string postId)
        {
            var post = _postService.GetPost(new Guid(postId));
            var viewModel = new PostDetailsViewModel {Post = post, Votes = post.Likes.Sum(x => x.Value)};
            return View(viewModel);
        }
        // POST: Post/Create

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Model.Content")] HomeIndexViewModel viewModel)
        {
            Post parentPost = null;
            if (Request.Form.Get("postModel.parentPost") != null)
            {
                var parentPostId = Request.Form.Get("postModel.ParentPost");
                parentPost = _postService.GetPost(new Guid(parentPostId));
            }

            var userId = User.Identity.GetUserId();
            var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));
            var content = Request.Form.Get("postModel.Content");
            _postService.AddPost(currentUserProfile, content, parentPost);
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
                if (Int32.TryParse(Request.Form["value"], out var value))
                {
                    value = value > 0 ? 1 : -1;
                    var postId = new Guid(Request.Form["post_id"]);
                    var currentUserProfile =
                        _userProfileService.GetUserProfileByUserId(new Guid(User.Identity.GetUserId()));
                    var post = _postService.GetPost(postId);
                    var existingLike = _likeService.GetLike(postId, currentUserProfile.Id);
                    if (existingLike == null)
                    {
                        _likeService.AddLike(currentUserProfile, value, post);
                        return Json(new {Message = "vote_registered"});
                    }

                    if (existingLike.Value != value)
                    {
                        existingLike.Value = value;
                        _likeService.ChangeLikeValue(existingLike.Id, value);
                        return Json(new {Message = "vote_registered"});
                    }

                    return Json(new {Message = "already_voted"});
                }
                else
                {
                    return Json(new {Message = "invalid_parameter"});
                }
            }
            else
            {
                return Json(new {Message = "invalid_parameter"});
            }
        }

        [HttpPost, ActionName("get-preview")]
        [Route("get-preview/", Name = "get-preview")]
        public async Task<JsonResult> GetLinkPreview()
        {
            var url = Request.Form["url"];
            if (url != null)
            {
                var result = _linkPreviewService.FindLinkPreview(url);
                if (result == null)
                {
                    result = await Task.Run(() => _linkPreviewService.GetUrlPreview(url));
                    try
                    {
                        _linkPreviewService.AddLinkPreviewInDb(result);
                    }
                    catch (Exception e)
                    {
                    }
                }

                return Json(result);
            }
            else
            {
                return Json("invalid url");
            }
        }
    }
}