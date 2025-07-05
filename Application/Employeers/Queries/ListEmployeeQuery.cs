using Application.Common.Attributes;
using Application.Common.DTO;
using Application.Common.Enums;
using Application.Common.Services;
using Application.Employeers.Queries.DTO;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Employeers.Queries;

public class ListEmployeeQuery : IRequest<PaginationDTO<GetEmployeeDTO>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;

    [Sortable] public string? Department { get; set; }
    [Sortable] public string? FirstName { get; set; }
    [Sortable] public string? MiddleName { get; set; }
    [Sortable] public string? LastName { get; set; }

    [Sortable] private DateTime? BirthDate { get; set; }
    [Sortable] private DateTime? HireDate { get; set; }
    [Sortable] private decimal? Salary { get; set; }

    public DateTime? MinBirthDate { get; set; }
    public DateTime? MaxBirthDate { get; set; }
    public DateTime? MinHireDate { get; set; }
    public DateTime? MaxHireDate { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public SortDirection SortDirection { get; set; } = SortDirection.None;
    public string? SortPropertyName { get; set; }
}

public class ListEmployeeQueryValidator : AbstractValidator<ListEmployeeQuery>
{
    private static readonly HashSet<string> _sortable =
    typeof(ListEmployeeQuery).GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(SortableAttribute)))
                    .Select(p => p.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public ListEmployeeQueryValidator()
    {
        RuleFor(query => query.SortPropertyName)
            .Must(_sortable.Contains)
            .When(query => query.SortPropertyName != null)
            .WithMessage($"{nameof(ListEmployeeQuery.SortPropertyName)} must be one of the following: {string.Join(", ", _sortable)}");
    }
}

public class ListEmployeeQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<ListEmployeeQuery, PaginationDTO<GetEmployeeDTO>>
{
    public async Task<PaginationDTO<GetEmployeeDTO>> Handle(ListEmployeeQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Employee> employees = db.Employees
            .Include(employee => employee.Department)
            .AsNoTrackingWithIdentityResolution();

        employees = FilterEmployees(employees, request);
        employees = SortEmployees(employees, request);

        var pagination = PaginationDTO<GetEmployeeDTO>.CreateMappedPagination(employees, request.Page, request.Limit, mapper);

        return pagination;
    }

    private static IQueryable<Employee> FilterEmployees(IQueryable<Employee> employees, ListEmployeeQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Department))
            employees = employees.Where(employee => employee.Department.Name.ToLower().Contains(request.Department.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            employees = employees.Where(employee => employee.FirstName.ToLower().Contains(request.FirstName.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.MiddleName))
            employees = employees
                .Where(employee => employee.MiddleName != null)
                .Where(employee => employee.MiddleName!.ToLower().Contains(request.MiddleName.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.LastName))
            employees = employees.Where(employee => employee.LastName.ToLower().Contains(request.LastName.ToLower()));

        if (request.MinBirthDate != null)
            employees = employees.Where(employee => employee.BirthDate >= request.MinBirthDate);

        if (request.MaxBirthDate != null)
            employees = employees.Where(employee => employee.BirthDate <= request.MaxBirthDate);

        if (request.MinHireDate != null)
            employees = employees.Where(employee => employee.HireDate >= request.MinHireDate);

        if (request.MaxHireDate != null)
            employees = employees.Where(employee => employee.HireDate <= request.MaxHireDate);

        if (request.MinSalary != null)
            employees = employees.Where(employee => employee.Salary >= request.MinSalary);

        if (request.MaxSalary != null)
            employees = employees.Where(employee => employee.Salary <= request.MaxSalary);

        return employees;
    }

    private static IQueryable<Employee> SortEmployees(IQueryable<Employee> employees, ListEmployeeQuery request)
    {
        if (request.SortPropertyName == null || request.SortDirection == SortDirection.None)
            return employees;

        if (request.SortDirection == SortDirection.Ascending)
            employees = employees.OrderBy(employee => employee.GetType().GetProperty(request.SortPropertyName));
        else
            employees = employees.OrderByDescending(employee => employee.GetType().GetProperty(request.SortPropertyName));

        return employees;
    }
}
