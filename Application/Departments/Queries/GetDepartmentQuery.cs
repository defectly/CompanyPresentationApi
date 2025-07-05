using Application.Common.Services;
using Application.Departments.Queries.DTO;
using Application.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Departments.Queries;

public record GetDepartmentQuery(Guid Id) : IRequest<GetDepartmentDTO>;

public class GetDepartmentQueryHandler(IDbContext db, IMapper mapper) : IRequestHandler<GetDepartmentQuery, GetDepartmentDTO>
{
    public async Task<GetDepartmentDTO> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
    {
        var department = await db.Departments.FirstOrDefaultAsync(department => department.Id == request.Id, cancellationToken);

        if (department == null)
            throw new NotFoundException($"Department with id {request.Id} not found");

        return mapper.Map<GetDepartmentDTO>(department);
    }
}