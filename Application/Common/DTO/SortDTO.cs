using Application.Common.Enums;

namespace Application.Common.DTO;

public class SortDTO
{
    public required string PropertyName { get; set; }
    public SortDirection SortDirection { get; set; }
}
