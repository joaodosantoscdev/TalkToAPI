using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class LinkDTO
    {
        public string Ref { get; set; }
        public string Method { get; set; }
        public string Href { get; set; }

        public LinkDTO(string @ref, string method, string href)
        {
            Ref = @ref;
            Method = method;
            Href = href;
        }
    }

}
