using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Image
    {
        [Required]
        public int Id { get; set; }

        public Guid UserProfileId { get; set; }

    }
}