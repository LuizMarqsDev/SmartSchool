using AutoMapper;
using SmartSchool.API.DTOS;
using SmartSchool.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSchool.API.Helpers
{
    public class SmartSchoolProfile : Profile
    {
        public SmartSchoolProfile()
        {
            CreateMap<Aluno, AlunoDto>().ForMember(dest => dest.Nome,
                                                   opt => opt.MapFrom(src => $"{src.Nome} {src.Sobrenome}"))
                                         .ForMember(dest => dest.Idade,
                                                    opt => opt.MapFrom(src => src.DataNascimento.GetCurrentAge()));

            CreateMap<AlunoDto, Aluno>();
            CreateMap<Aluno, AlunoRegistratDto>().ReverseMap();

            CreateMap<Professor, ProfessorDto>().ForMember(dest => dest.Nome,
                                                  opt => opt.MapFrom(src => $"{src.Nome} {src.Sobrenome}"));



            CreateMap<ProfessorDto, Professor>();
            CreateMap<Professor, ProfessorRegistrarDto>().ReverseMap();

        }
    }
}
