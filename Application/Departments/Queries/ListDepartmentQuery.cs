using Application.Common.Attributes;
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

    public SortDirection SortDirection { get; set; } = SortDirection.None;
    public string? SortPropertyName { get; set; }
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
        RuleFor(query => query.SortPropertyName)
            .Must(sortPropertyName => _sortable.Contains(sortPropertyName!))
            .When(sort => sort.SortPropertyName != null)
            .WithMessage($"{nameof(ListDepartmentQuery.SortPropertyName)} must be one of the following: {string.Join(", ", _sortable)}");
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
        if (request.SortPropertyName == null || request.SortDirection == SortDirection.None)
            return departments;

        if (request.SortDirection == SortDirection.Ascending)
            departments = departments.OrderBy(employee => employee.GetType().GetProperty(request.SortPropertyName));
        else
            departments = departments.OrderByDescending(employee => employee.GetType().GetProperty(request.SortPropertyName));

        return departments;
    }
}
