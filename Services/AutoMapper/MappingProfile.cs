using AutoMapper;
using BusinessObjects.Dtos.User;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Dtos.Comment;

namespace Services.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Entity to DTO
            CreateMap<User, UserInformationDto>();

            //DTO to Entity
            CreateMap<MangaBoxDto, MangaBox>()
                      .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UserCollectionDto, UserCollection>()
                     .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CommentCreateDto, Comment>()
         .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
