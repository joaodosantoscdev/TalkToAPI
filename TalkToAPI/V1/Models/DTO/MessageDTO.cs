using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class MessageDTO : BaseDTO
    {
        public int Id { get; set; }      
        public ApplicationUser Owner { get; set; }      
        public string OwnerId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public string ReceiverId { get; set; }
        public string ChatMessage { get; set; }
        public bool Excluded { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Att { get; set; }
    }
}
