using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class LinkPreview
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }
}