using Domain.Entities.BaseEntities;

namespace Domain.Entities;

public class Department : BaseEntityTimeStamp
{
    public required string Name { get; set; }
    public List<Employee> Employees { get; set; } = [];
}
