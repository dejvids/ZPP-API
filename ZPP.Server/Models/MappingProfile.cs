using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Dtos;
using ZPP.Server.Entities;

namespace ZPP.Server.Models
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Lecture, LectureDto>();
            CreateMap<LectureDto, Lecture>();
            CreateMap<NewLectureDto, Lecture>();
            CreateMap<Opinion, OpinionDto>()
                .ForMember(destination => destination.LectureName, opts => opts.MapFrom(source => source.Lecture.Name));
            CreateMap<OpinionDto, Opinion>();
            CreateMap<NewOpinionDto, Opinion>();
        }
    }
}
