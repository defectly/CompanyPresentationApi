namespace Presentation.DTO;

public class UpdateEmployeeDTO
{
    public string? Department { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }
}
