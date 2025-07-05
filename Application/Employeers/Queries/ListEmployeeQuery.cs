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

    public string? Department { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }

    public DateTime? MinBirthDate { get; set; }
    public DateTime? MaxBirthDate { get; set; }
    public DateTime? MinHireDate { get; set; }
    public DateTime? MaxHireDate { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public SortDirection SortDirection { get; set; } = SortDirection.None;
    public EmployeeSort EmployeeSort { get; set; } = EmployeeSort.None;
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
        if (request.EmployeeSort == EmployeeSort.None || request.SortDirection == SortDirection.None)
            return employees;

        employees = request.EmployeeSort switch
        {
            EmployeeSort.Department => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.Department.Name) :
                employees.OrderByDescending(employee => employee.Department.Name),

            EmployeeSort.FirstName => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.FirstName) :
                employees.OrderByDescending(employee => employee.FirstName),

            EmployeeSort.MiddleName => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.MiddleName) :
                employees.OrderByDescending(employee => employee.MiddleName),

            EmployeeSort.LastName => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.LastName) :
                employees.OrderByDescending(employee => employee.LastName),

            EmployeeSort.BirthDate => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.BirthDate) :
                employees.OrderByDescending(employee => employee.BirthDate),

            EmployeeSort.HireDate => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.HireDate) :
                employees.OrderByDescending(employee => employee.HireDate),

            EmployeeSort.Salary => employees = request.SortDirection == SortDirection.Ascending ?
                employees.OrderBy(employee => employee.Salary) :
                employees.OrderByDescending(employee => employee.Salary),

            _ => employees
        };

        return employees;
    }
}
