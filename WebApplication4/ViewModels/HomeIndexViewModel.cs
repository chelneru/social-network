using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{

    public class HomeIndexViewModel
    {
        public List<HomeIndexPostViewModel> Posts { get; set; }
        public Post PostModel { get; set; }


        public HomeIndexViewModel()
        {
            Posts = new List<HomeIndexPostViewModel>();
            PostModel = new Post();

        }
    }
}