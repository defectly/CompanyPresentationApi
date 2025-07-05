using Application.Common.Attributes;
using Application.Common.DTO;
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
    [Sortable] public string? Name { get; set; }

    public SortDTO[] Sorts { get; set; } = [];
}

public class ListDepartmentQueryValidator : AbstractValidator<ListDepartmentQuery>
{
    private static readonly HashSet<string> _sortable =
    typeof(ListDepartmentQuery).GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(SortableAttribute)))
                    .Select(p => p.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public ListDepartmentQueryValidator()
    {
        RuleForEach(query => query.Sorts)
            .Must(sort => _sortable.Contains(sort.PropertyName));
    }
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
        string[] properties = typeof(ListDepartmentQuery).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(SortableAttribute)))
            .Select(property => property.Name).ToArray();

        for (int i = 0; i < request.Sorts.Length; i++)
        {
            if (request.Sorts[i] == null)
                continue;

            if (request.Sorts[i].SortDirection == SortDirection.Ascending)
                departments = departments.OrderBy(employee => employee.GetType().GetProperty(request.Sorts[i].PropertyName));
            else
                departments = departments.OrderByDescending(employee => employee.GetType().GetProperty(request.Sorts[i].PropertyName));
        }

        return departments;
    }
}
