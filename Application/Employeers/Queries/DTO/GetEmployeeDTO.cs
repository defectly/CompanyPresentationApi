using AutoMapper;
using Domain.Entities;

namespace Application.Employeers.Queries.DTO;
public class GetEmployeeDTO
{
    public required Guid Id { get; set; }
    public required GetEmployeeDepartmentDTO Department { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required DateTime HireDate { get; set; }
    public required decimal Salary { get; set; }
}

public class GetEmployeeDepartmentDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}

public class GetAboutCompanyMappingProfile : Profile
{
    public GetAboutCompanyMappingProfile()
    {
        CreateMap<Employee, GetEmployeeDTO>()
            .ForMember(src => src.Department, dest => dest.MapFrom(employee => employee.Department));

        CreateMap<Department, GetEmployeeDepartmentDTO>();
    }
}
