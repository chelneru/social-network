using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using WebApplication4.DAL;
using WebApplication4.Models;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace WebApplication4.Services
{
    public class LikeService : BaseService
    {
     
        public LikeService()
        {

        }
        public static Like GetLike(Guid postId, Guid posterId)
        {
            var like = Context.Like.Include(l => l.Post )
                .FirstOrDefault(x => x.Post.Id == postId && x.UserProfileId == posterId);

            return like;
        }

        public static bool AddLike(UserProfile poster, Int32 value, Post post)
        {
            var like = new Like
            {
                Id = Guid.NewGuid(),
                UserProfile = poster,
                UserProfileId = poster.Id,
                Value = value,
                Post = post,
                PostId = post.Id
            };

            var context = new ValidationContext(like, null, null);
            var results = new List<ValidationResult>();
            if (Validator.TryValidateObject(like, context, results, true))
            {
                Context.Like.Add(like);
                Context.SaveChanges();
                return true;
            }

            System.Diagnostics.Debug.WriteLine("Validation error for a like");
            return false;
        }

        public static bool ChangeLikeValue(Guid likeId, Int32 value)
        {
             var like = Context.Like.FirstOrDefault(x => x.Id == likeId);
            if(like != null) {
            like.Value = value;
                Context.SaveChanges();
                return true;
            }
            System.Diagnostics.Debug.WriteLine("Like with GUID:" + likeId +" doesn't exist !");
            return true;
        }
    }
}