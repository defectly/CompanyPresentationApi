using Domain.Entities.BaseEntities;

namespace Domain.Entities;

public class Employee : BaseEntityTimeStamp
{
    public required Department Department { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required DateTime HireDate { get; set; }
    public required decimal Salary { get; set; }
}
