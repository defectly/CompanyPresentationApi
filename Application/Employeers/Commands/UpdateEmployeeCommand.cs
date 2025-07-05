using Application.Common.Services;
using Application.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Employeers.Commands;

public class UpdateEmployeeCommand : IRequest
{
    public required Guid Id { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }
}

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        // FirstName validation
        RuleFor(p => p.FirstName)
            .NotEmpty().When(p => p.FirstName != null).WithMessage("First name should not be empty")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("First name can only contain letters, apostrophes, or hyphens");

        // MiddleName validation - only applied when not null
        RuleFor(p => p.MiddleName)
            .Length(2, 50).When(p => p.MiddleName != null).WithMessage("Middle name must be between 2 and 50 characters when provided")
            .Matches(@"^[a-zA-Z'-]+$").When(p => p.MiddleName != null).WithMessage("Middle name can only contain letters, apostrophes, or hyphens when provided");

        // LastName validation - only applied when not null
        RuleFor(p => p.LastName)
            .NotEmpty().When(p => p.LastName != null).WithMessage("Last name should not be empty")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("Last name can only contain letters, apostrophes, or hyphens");
    }
}


public class UpdateEmployeeCommandHandler(IDbContext db) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(employee => employee.Id == request.Id, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with id {request.Id} not found");

        if (request.DepartmentId != null)
        {
            var department = await db.Departments.FirstOrDefaultAsync(department => department.Id == request.DepartmentId, cancellationToken);

            if (department == null)
                throw new NotFoundException($"Department with id {request.DepartmentId} not found");

            employee.Department = department;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            employee.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.MiddleName))
            employee.MiddleName = request.MiddleName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            employee.LastName = request.LastName;

        if (request.BirthDate != null)
            employee.BirthDate = (DateTime)request.BirthDate;

        if (request.HireDate != null)
            employee.HireDate = (DateTime)request.HireDate;

        if (request.Salary != null)
            employee.Salary = (decimal)request.Salary;

        employee.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }
}
