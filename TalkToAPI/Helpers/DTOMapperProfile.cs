using AutoMapper;
using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models.DTO;

namespace TalkToAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Name, orig => orig.MapFrom(src => src.FullName));

            CreateMap<Message, MessageDTO>();
            /*CreateMap<PaginationList<Word>, PaginationList<WordDTO>>();*/
        }
    }
}
