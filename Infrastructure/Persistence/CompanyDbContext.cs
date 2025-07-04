using Application.Common.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class CompanyDbContext(DbContextOptions options) : DbContext(options), IDbContext
{
    public DbSet<Employee> Employees { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}
