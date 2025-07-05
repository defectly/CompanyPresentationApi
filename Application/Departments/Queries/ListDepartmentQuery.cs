using Application.Common.Enums;
using Application.Common.Services;
using Application.Departments.Queries.DTO;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Departments.Queries;

public class ListDepartmentQuery : IRequest<List<GetDepartmentDTO>>
{
    public string? Name { get; set; }

    public DepartmentSort DepartmentSort { get; set; } = DepartmentSort.None;
    public SortDirection SortDirection { get; set; } = SortDirection.None;
}

public class ListDepartmentQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<ListDepartmentQuery, List<GetDepartmentDTO>>
{
    public async Task<List<GetDepartmentDTO>> Handle(ListDepartmentQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Department> departments = db.Departments
            .AsNoTrackingWithIdentityResolution();

        departments = FilterDepartments(departments, request);
        departments = SortDepartments(departments, request);

        var departmentsDTO = mapper.Map<List<GetDepartmentDTO>>(departments);

        return departmentsDTO;
    }

    private static IQueryable<Department> FilterDepartments(IQueryable<Department> Departments, ListDepartmentQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
            Departments = Departments.Where(Department => Department.Name.ToLower().Contains(request.Name.ToLower()));

        return Departments;
    }

    private static IQueryable<Department> SortDepartments(IQueryable<Department> departments, ListDepartmentQuery request)
    {
        if (request.DepartmentSort == DepartmentSort.None || request.SortDirection == SortDirection.None)
            return departments;

        departments = request.DepartmentSort switch
        {
            DepartmentSort.Name => departments = request.SortDirection == SortDirection.Ascending ?
                departments.OrderBy(department => department.Name) :
                departments.OrderByDescending(department => department.Name),

            _ => departments
        };

        return departments;
    }
}
