using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication4.Models
{
    public class Like
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        [Required]
        [ForeignKey("Post")]
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        [Required]
        public int Value { get; set; }
    }
}