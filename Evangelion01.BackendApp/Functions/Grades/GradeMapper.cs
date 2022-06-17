using AutoMapper;
using Evangelion01.Contracts.Models;
using System;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public class GradeMapperProfile : Profile
    {
        public GradeMapperProfile()
        {
            CreateMap<Grade, GradeDto>();
            CreateMap<AddGradeModel, Grade>()
                .ForMember(x => x.GradeId, opt => opt.MapFrom((s, d) => Guid.NewGuid().ToString()));
        }
    }
}
