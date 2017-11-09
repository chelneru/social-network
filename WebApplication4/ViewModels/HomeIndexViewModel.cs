using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Post> Posts { get; set; }
        public Post Model { get; set; }

        public HomeIndexViewModel(List<Post> posts)
        {
            Posts = posts;
            Model = new Post();

        }
    }
}