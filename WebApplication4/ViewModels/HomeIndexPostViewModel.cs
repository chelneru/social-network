using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class HomeIndexPostViewModel
    {
        public Guid Id { get; set; }
        public bool Edited { get; set; }
        public DateTime PostDateTime { get; set; }
        public string Content { get; set; }
        public Post ParentPost { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public int Likes { get; set; }
        public short CurrentUserVote { get; set; }
        public string PhotoLink { get; set; }
        public string VideoLink { get; set; }
        public string ShareLink { get; set; }

    }
}