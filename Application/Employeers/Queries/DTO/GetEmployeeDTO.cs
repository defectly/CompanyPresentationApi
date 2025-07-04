using AutoMapper;
using Domain.Entities;

namespace Application.Employeers.Queries.DTO;
public class GetEmployeeDTO
{
    public required Guid Id { get; set; }
    public required string Department { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required DateTime HireDate { get; set; }
    public required decimal Salary { get; set; }
}

public class GetAboutCompanyMappingProfile : Profile
{
    public GetAboutCompanyMappingProfile()
    {
        CreateMap<Employee, GetEmployeeDTO>();
    }
}
