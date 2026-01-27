using AutoMapper;
using Chronos.Application.DTOs.Department;
using Chronos.Application.DTOs.Employee;
using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map từ Entity -> DTO (Read)
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<CreateEmployeeDto, Employee>();

            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            CreateMap<EmploymentContractDto, EmploymentContract>().ReverseMap();
            CreateMap<EmploymentContractListDto, EmploymentContract>().ReverseMap().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee.EmployeeCode)); ;
            CreateMap<EmploymentContract,CreateEmploymentContractDto>().ReverseMap();

        }
    }
}
