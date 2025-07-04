using Application.Common.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.Employeers.Commands;

public class CreateEmployeeCommand : IRequest<Guid>
{
    public required string Department { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required DateTime HireDate { get; set; }
    public required decimal Salary { get; set; }
}

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        // Department validation
        RuleFor(x => x.Department)
            .Must(department => !string.IsNullOrWhiteSpace(department));

        // FirstName validation
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("First name can only contain letters, apostrophes, or hyphens");

        // MiddleName validation - only applied when not null
        RuleFor(p => p.MiddleName)
            .Length(2, 50).When(p => p.MiddleName != null).WithMessage("Middle name must be between 2 and 50 characters when provided")
            .Matches(@"^[a-zA-Z'-]+$").When(p => p.MiddleName != null).WithMessage("Middle name can only contain letters, apostrophes, or hyphens when provided");

        // LastName validation
        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("Last name can only contain letters, apostrophes, or hyphens");
    }
}

public class CreateEmployeeCommandHandler(IDbContext db) : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            Department = request.Department,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            HireDate = request.HireDate,
            Salary = request.Salary,
        };

        await db.Employees.AddAsync(employee, cancellationToken);

        await db.SaveChangesAsync();

        return employee.Id;
    }
}
