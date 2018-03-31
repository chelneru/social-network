using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public Post Post { get; set; }
        public int Votes { get; set; }
        public int CurrentUserVote { get; set; }
        public ICollection<Post> Comments { get; set; }
    }
}