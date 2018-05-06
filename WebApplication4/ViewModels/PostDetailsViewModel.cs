using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class PostDetailsViewModel
    {
        public List<HomeIndexPostViewModel> Posts { get; set; }
        public HomeIndexPostViewModel CurrentPost { get; set; }
        public Post PostModel { get; set; }
    }
}