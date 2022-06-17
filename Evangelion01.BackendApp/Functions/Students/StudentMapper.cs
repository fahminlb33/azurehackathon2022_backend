using AutoMapper;
using Evangelion01.Contracts.Models;
using System;

namespace Evangelion01.BackendApp.Functions.Students
{
    public class StudentMapperProfile : Profile
    {
        public StudentMapperProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<AddStudentModel, Student>()
                .ForMember(x => x.StudentId, opt => opt.MapFrom((s, d) => Guid.NewGuid().ToString()));
        }
    }
}
