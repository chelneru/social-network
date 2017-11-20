using System;
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
        [Required]
        public DateTime BirthDate { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}