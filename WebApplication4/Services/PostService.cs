using System;
using WebApplication4.DAL;
using WebApplication4.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using WebApplication4.DAL.Interfaces;
using WebApplication4.ViewModels;

namespace WebApplication4.Services
{
    public class PostService : BaseService
    {

        public PostService()
        {
            
        }
        public static List<HomeIndexPostViewModel> GetPosts()
        {
            var posts = Context.Post
                .Include(p => p.UserProfile)
                .Include(p => p.Likes)
                .Select(p => new HomeIndexPostViewModel()
                {
                    Id = p.Id,
                    Edited = p.Edited,
                    PostDateTime = p.PostDateTime,
                    Content = p.Content,
                    ParentPost = p.ParentPost,
                    UserAddress = p.UserProfile.UserAddress,
                    UserName = p.UserProfile.Name,
                    PhotoLink = p.PhotoLink,
                    VideoLink = p.VideoLink,
                    ShareLink = p.ShareLink,
                    LinkPreview = Context.LinkPreview.Where(x => x.Url == p.ShareLink).FirstOrDefault(),
                Likes = p.Likes.Sum(l => l.Value) == null ? 0: p.Likes.Sum(l => l.Value),
                    CurrentUserVote = p.Likes.Where(l => l.UserProfile.Id == p.UserProfile.Id).Select(l => l.Value).FirstOrDefault()
                }).OrderByDescending(x => x.PostDateTime).ToList();


            return posts;
        }

        public static Post GetPost(Guid id)
        {
            var post = Context.Post
                .Include(p => p.UserProfile)
                .Include(p => p.Likes)
                .First(x => x.Id == id);
            return post;
        }

        public static List<Post> GetComments(Guid postId)
        {
            var comments = Context.Post
                .Include(p => p.UserProfile)
                .Where(x => x.ParentPost.Id == postId)
                .ToList();
            return comments;
        }

        public static List<Post> GetUserPhotos(Guid id) 
        {
            return Context.Post
                .Where(x => x.UserProfileId == id && x.PhotoLink != null)
                .ToList();

        }

        public static List<Post> SearchPostsByContent(string searchString)
        {
            return Context.Post
                .Where(x => x.Content.Contains(searchString))
                .Select(x => new Post(){ Id = x.Id, Content = x.Content})
                .ToList();
        }
        public static bool AddPost(UserProfile poster, string content, Post parentPost = null, string link = null,
            string imageLink = null, string videoLink = null)
        {
            var post = new Post
            {
                UserProfileId = poster.Id,
                UserProfile = poster,
                Content = content,
                PostDateTime = DateTime.Now,
                ParentPost = parentPost,
                PhotoLink = imageLink,
                VideoLink = videoLink,
                ShareLink = link,
                Id = Guid.NewGuid()
            };
            var context = new ValidationContext(post, null, null);
            var results = new List<ValidationResult>();
            if (Validator.TryValidateObject(post, context, results, true))
            {
                Context.Post.Add(post);
                Context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}