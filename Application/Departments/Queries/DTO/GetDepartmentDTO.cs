using AutoMapper;
using Domain.Entities;

namespace Application.Departments.Queries.DTO;

public class GetDepartmentDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}

public class GetDepartmentMappingProfile : Profile
{
    public GetDepartmentMappingProfile()
    {
        CreateMap<Department, GetDepartmentDTO>();
    }
}
