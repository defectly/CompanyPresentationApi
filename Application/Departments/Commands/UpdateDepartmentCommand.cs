using Application.Common.Services;
using Application.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Departments.Commands;

public class UpdateDepartmentCommand : IRequest
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
}

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        // FirstName validation
        RuleFor(p => p.Name)
            .NotEmpty().When(p => p.Name != null).WithMessage("First name should not be empty")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("First name can only contain letters, apostrophes, or hyphens");
    }
}

public class UpdateDepartmentCommandHandler(IDbContext db) : IRequestHandler<UpdateDepartmentCommand>
{
    public async Task Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await db.Employees.FirstOrDefaultAsync(employee => employee.Id == request.Id, cancellationToken);

        if (department == null)
            throw new NotFoundException($"Department with id {request.Id} not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            department.FirstName = request.Name;

        await db.SaveChangesAsync();
    }
}
