using Application.Common.Services;
using Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Employeers.Commands;

public record DeleteEmployeeCommand(Guid Id) : IRequest;

public class DeleteEmployeeCommandHandler(IDbContext db) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(employee => employee.Id == request.Id, cancellationToken);

        if (employee == null)
            throw new NotFoundException();

        db.Employees.Remove(employee);

        await db.SaveChangesAsync();
    }
}
