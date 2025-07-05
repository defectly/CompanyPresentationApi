using Application.Common.DTO;
using Application.Common.Enums;
using Application.Common.Services;
using Application.Departments.Queries.DTO;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Departments.Queries;

public class ListDepartmentQuery : IRequest<PaginationDTO<GetDepartmentDTO>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;

    public string? Name { get; set; }

    public NameSort NameSort { get; set; } = NameSort.None;
}

public class ListDepartmentQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<ListDepartmentQuery, PaginationDTO<GetDepartmentDTO>>
{
    public async Task<PaginationDTO<GetDepartmentDTO>> Handle(ListDepartmentQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Department> departments = db.Departments
            .AsNoTrackingWithIdentityResolution();

        departments = FilterDepartments(departments, request);
        departments = SortDepartments(departments, request);

        var pagination = PaginationDTO<GetDepartmentDTO>.CreateMappedPagination(departments, request.Page, request.Limit, mapper);

        return pagination;
    }

    private static IQueryable<Department> FilterDepartments(IQueryable<Department> Departments, ListDepartmentQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
            Departments = Departments.Where(Department => Department.Name.ToLower().Contains(request.Name.ToLower()));

        return Departments;
    }

    private static IQueryable<Department> SortDepartments(IQueryable<Department> Departments, ListDepartmentQuery request)
    {
        if (request.NameSort != NameSort.None)
            Departments = request.NameSort == NameSort.Ascending ? Departments.OrderBy(dep => dep.Name) : Departments.OrderByDescending(dep => dep.Name);

        return Departments;
    }
}
