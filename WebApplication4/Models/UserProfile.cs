using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }
        public string Location { get; set; }
        public string AvatarUrl { get; set; }
        public string About { get; set; }
        [RegularExpression("^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}.(xn--)?([a-z0-9]{1,61}|[a-z0-9-]{1,30}.[a-z]{1,})$", ErrorMessage = "Invalid User Address Name")]
        public string UserAddress { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public ApplicationUser User { get; set; }
        public virtual ICollection<Post> UserPosts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}