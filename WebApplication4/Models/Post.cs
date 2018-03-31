using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; }
        public bool Edited { get; set; }
        [Required]
        public DateTime PostDateTime { get; set; }
        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual Post ParentPost { get; set; }
        public string PhotoLink { get; set; }
        public string VideoLink { get; set; }
        public string ShareLink { get; set; }

    }
}