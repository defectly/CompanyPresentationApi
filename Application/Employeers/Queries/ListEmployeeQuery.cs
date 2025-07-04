using Application.Common.DTO;
using Application.Common.Enums;
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

    public DepartmentSort DepartmentSort { get; set; } = DepartmentSort.None;
    public FirstNameSort FirstNameSort { get; set; } = FirstNameSort.None;
    public MiddleNameSort MiddleNameSort { get; set; } = MiddleNameSort.None;
    public LastNameSort LastNameSort { get; set; } = LastNameSort.None;
    public HireDateSort HireDateSort { get; set; } = HireDateSort.None;
    public BirthDateSort BirthDateSort { get; set; } = BirthDateSort.None;
    public SalarySort SalarySort { get; set; } = SalarySort.None;
}

public class ListEmployeeQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<ListEmployeeQuery, PaginationDTO<GetEmployeeDTO>>
{
    public async Task<PaginationDTO<GetEmployeeDTO>> Handle(ListEmployeeQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Employee> employees = db.Employees
            .AsNoTrackingWithIdentityResolution();

        FilterEmployees(employees, request);
        SortEmployees(employees, request);

        var pagination = PaginationDTO<GetEmployeeDTO>.CreateMappedPagination(employees, request.Page, request.Limit, mapper);

        return pagination;
    }

    private static void FilterEmployees(IQueryable<Employee> employees, ListEmployeeQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Department))
            employees = employees.Where(employee => employee.Department.ToLower().Contains(request.Department.ToLower()));

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
    }

    private static void SortEmployees(IQueryable<Employee> employees, ListEmployeeQuery request)
    {
        if (request.DepartmentSort != DepartmentSort.None)
            employees = request.DepartmentSort == DepartmentSort.Ascending ? employees.OrderBy(emp => emp.Department) : employees.OrderByDescending(emp => emp.Department);

        if (request.FirstNameSort != FirstNameSort.None)
            employees = request.FirstNameSort == FirstNameSort.Ascending ? employees.OrderBy(emp => emp.FirstName) : employees.OrderByDescending(emp => emp.FirstName);

        if (request.MiddleNameSort != MiddleNameSort.None)
            employees = request.MiddleNameSort == MiddleNameSort.Ascending ? employees.OrderBy(emp => emp.MiddleName) : employees.OrderByDescending(emp => emp.MiddleName);

        if (request.LastNameSort != LastNameSort.None)
            employees = request.LastNameSort == LastNameSort.Ascending ? employees.OrderBy(emp => emp.LastName) : employees.OrderByDescending(emp => emp.LastName);

        if (request.HireDateSort != HireDateSort.None)
            employees = request.HireDateSort == HireDateSort.Ascending ? employees.OrderBy(emp => emp.HireDate) : employees.OrderByDescending(emp => emp.HireDate);

        if (request.BirthDateSort != BirthDateSort.None)
            employees = request.BirthDateSort == BirthDateSort.Ascending ? employees.OrderBy(emp => emp.BirthDate) : employees.OrderByDescending(emp => emp.BirthDate);

        if (request.SalarySort != SalarySort.None)
            employees = request.SalarySort == SalarySort.Ascending ? employees.OrderBy(emp => emp.Salary) : employees.OrderByDescending(emp => emp.Salary);
    }
}
