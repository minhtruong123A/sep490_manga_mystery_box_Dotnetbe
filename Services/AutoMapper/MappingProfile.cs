using AutoMapper;
using BusinessObjects.Dtos.User;
using BusinessObjects;
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
            //Entity to DTO
            CreateMap<User, UserInformationDto>();

            //DTO to Entity
        }
    }
}
