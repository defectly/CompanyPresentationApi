using Application.Common.Services;
using Application.Employeers.Queries.DTO;
using Application.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Employeers.Queries;

public record GetEmployeeQuery(Guid Id) : IRequest<GetEmployeeDTO>;

public class GetEmployeeQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<GetEmployeeQuery, GetEmployeeDTO>
{
    public async Task<GetEmployeeDTO> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = db.Employees.FirstOrDefaultAsync(employee => employee.Id == request.Id, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with id {request.Id} not found");

        return mapper.Map<GetEmployeeDTO>(employee);
    }
}