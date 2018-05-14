using System.Collections.Generic;
using WebApplication4.Models;

namespace WebApplication4.ViewModels
{
    public class GalleryViewModel
    {
        public List<WebApplication4.Models.Post> Posts { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}