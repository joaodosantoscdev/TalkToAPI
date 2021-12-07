using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class UserDTONoHyperLink
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Slogan { get; set; }
    }
}
