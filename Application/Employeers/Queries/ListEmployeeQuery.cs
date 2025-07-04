using Application.Common.DTO;
using Application.Common.Services;
using Application.Employeers.Queries.DTO;
using AutoMapper;
using Domain.Entities;
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
    public DateTime? MinBirthDate { get; set; }
    public DateTime? MaxBirthDate { get; set; }
    public DateTime? MinHireDate { get; set; }
    public DateTime? MaxHireDate { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}

public class ListEmployeeQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<ListEmployeeQuery, PaginationDTO<GetEmployeeDTO>>
{
    public async Task<PaginationDTO<GetEmployeeDTO>> Handle(ListEmployeeQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Employee> employees = db.Employees
            .AsNoTrackingWithIdentityResolution();

        FilterEmployees(employees, request);

        var pagination = PaginationDTO<GetEmployeeDTO>.CreateMappedPagination(employees, request.Page, request.Limit, mapper);

        return pagination;
    }

    private static void FilterEmployees(IQueryable<Employee> employees, ListEmployeeQuery request)
    {
        if (request.Department != null)
            employees = employees.Where(employee => employee.Department.ToLower().Contains(request.Department.ToLower()));

        if (request.FirstName != null)
            employees = employees.Where(employee => employee.FirstName.ToLower().Contains(request.FirstName.ToLower()));

        if (request.MiddleName != null)
            employees = employees
                .Where(employee => employee.MiddleName != null)
                .Where(employee => employee.MiddleName!.ToLower().Contains(request.MiddleName.ToLower()));

        if (request.LastName != null)
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
    }
}