using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models.DTO
{
    public class ListDTO<T> : BaseDTO
    {
        public List<T> List { get; set; }
    }
}
