using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public int Votes { get; set; }
    }
}