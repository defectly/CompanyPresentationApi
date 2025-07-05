using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Services;

public interface IDbContext
{
    DbSet<Employee> Employees { get; set; }
    DbSet<Department> Departments { get; set; }

    Task<int> SaveChangesAsync();
}
