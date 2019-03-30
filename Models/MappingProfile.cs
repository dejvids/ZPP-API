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
            CreateMap<Company, CompanyDto>();
            CreateMap<CompanyDto, Company>();
            CreateMap<NewCompanyDto, Company>();
            CreateMap<User, UserDetailDto>()
                .ForMember(destination => destination.CompanyName, opts => opts.MapFrom(source => source.Company.Name));

            CreateMap<Participant, ParticipantDto>()
                .ForMember(destination => destination.Login, opts => opts.MapFrom(source => source.Student.Login))
                .ForMember(destination => destination.Name, opts => opts.MapFrom(source => source.Student.Name))
                .ForMember(destination => destination.Surname, opts => opts.MapFrom(source => source.Student.Surname))
                .ForMember(destination => destination.Email, opts => opts.MapFrom(source => source.Student.Email));

            CreateMap<NewParticipantDto, Participant>();
        }
    }
}
