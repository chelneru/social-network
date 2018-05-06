using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using WebApplication4.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    public class PostController : Controller
    {
//        public ActionResult Index()
//        {
//            var list = PostService.GetPosts();
//            return View(list);
//        }

        [Route("posts/{postId}/", Name = "posts")]
        public ActionResult Details(string postId)
        {
            var viewModel = new PostDetailsViewModel
            {
                CurrentPost = PostService.GetDetailedPostInfo(new Guid(postId)),
                Posts = PostService.GetPostComments(new Guid(postId)),
                PostModel = new Post()
            };
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
                parentPost = PostService.GetPost(new Guid(parentPostId));
            }
            var content = Request.Form.Get("postModel.Content");
            string url = null;
            if (Request.Form.Get("lpid") != null)
            {
                var lpid = new Guid(Request.Form.Get("lpid"));

                var link = LinkPreviewService.FindLinkPreviewById(lpid);
                content = link.Url;
                url = link.Url;
            }

            var userId = User.Identity.GetUserId();
            var currentUserProfile = UserProfileService.GetUserProfileByUserId(new Guid(userId));
            PostService.AddPost(currentUserProfile, content, parentPost,url);
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
                        UserProfileService.GetUserProfileByUserId(new Guid(User.Identity.GetUserId()));
                    var post = PostService.GetPost(postId);
                    var existingLike = LikeService.GetLike(postId, currentUserProfile.Id);
                    if (existingLike == null)
                    {
                        LikeService.AddLike(currentUserProfile, value, post);
                        return Json(new {Message = "vote_registered"});
                    }

                    if (existingLike.Value != value)
                    {
                        existingLike.Value = value;
                        LikeService.ChangeLikeValue(existingLike.Id, value);
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
                var result = LinkPreviewService.FindLinkPreview(url);
                if (result == null)
                {
                    result = await Task.Run(() => LinkPreviewService.GetUrlPreview(url));
                    try
                    {
                        LinkPreviewService.AddLinkPreviewInDb(result);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
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