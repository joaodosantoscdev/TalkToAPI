using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [Required]
        public string ChatMessage { get; set; }
        public DateTime Created { get; set; }

    }
}
