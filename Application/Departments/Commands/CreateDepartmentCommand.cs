using Application.Common.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Departments.Commands;

public class CreateDepartmentCommand : IRequest<Guid>
{
    public required string Name { get; set; }
}

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        // FirstName validation
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z'-]+$").WithMessage("Name can only contain letters, apostrophes, or hyphens");
    }
}

public class CreateDepartmentCommandHandler(IDbContext db) : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Name = request.Name,
        };

        await db.Departments.AddAsync(department, cancellationToken);

        await db.SaveChangesAsync();

        return department.Id;
    }
}
