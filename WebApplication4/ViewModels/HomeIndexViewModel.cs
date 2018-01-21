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
        public Post postModel { get; set; }


        public HomeIndexViewModel()
        {
            Posts = new List<Post>();
            postModel = new Post();

        }
    }
}