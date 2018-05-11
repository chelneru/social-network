using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class PrivateMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid InitiatorUserProfileId { get; set; }
        public Guid TargetUserProfileId { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Seen { get; set; }
    }
}