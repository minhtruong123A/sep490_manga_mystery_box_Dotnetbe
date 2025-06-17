using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Comment;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.User;
using BusinessObjects.Dtos.UserBox;
using BusinessObjects.Dtos.UserCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO
            CreateMap<User, UserInformationDto>();
            CreateMap<UserBox, UserBoxGetAllDto>()
                .ForMember(dest => dest.BoxTitle, opt => opt.Ignore());

            CreateMap<UserCollection, UserCollectionGetAllDto>()
                .ForMember(dest => dest.CollectionTopic, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Count, opt => opt.Ignore());

            // DTO to Entity
            CreateMap<MangaBoxDto, MangaBox>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UserCollectionDto, UserCollection>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CommentCreateDto, Comment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
