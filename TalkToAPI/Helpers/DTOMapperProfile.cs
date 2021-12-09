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

            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.FullName, orig => orig.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserName, orig => orig.MapFrom(src => src.Email))
                .ForMember(dest => dest.Id, orig => orig.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserDTONoHyperLink>()
                .ForMember(dest => dest.Name, orig => orig.MapFrom(src => src.FullName));

            CreateMap<Message, MessageDTO>();
            /*CreateMap<PaginationList<Word>, PaginationList<WordDTO>>();*/
        }
    }
}
