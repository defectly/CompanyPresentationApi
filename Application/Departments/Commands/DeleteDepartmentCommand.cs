using Application.Common.Services;
using Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Employeers.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest;

public class DeleteDepartmentCommandHandler(IDbContext db) : IRequestHandler<DeleteDepartmentCommand>
{
    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await db.Departments.FirstOrDefaultAsync(employee => employee.Id == request.Id, cancellationToken);

        if (department == null)
            throw new NotFoundException($"Department with id {request.Id} not found");

        db.Departments.Remove(department);

        await db.SaveChangesAsync();
    }
}
