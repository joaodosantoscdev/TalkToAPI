using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Used { get; set; }
        public DateTime ExpirationToken { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Att { get; set; }
    }
}
