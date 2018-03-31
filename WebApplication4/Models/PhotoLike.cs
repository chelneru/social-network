using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication4.Models
{
    public class PhotoLike
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; } 
        public UserProfile UserProfile { get; set; }
        [Required]
        [ForeignKey("Photo")]
        public Guid PhotoId { get; set; }
        public Photo Photo { get; set; }
        [Required]
        public int Value { get; set; }
    }
}