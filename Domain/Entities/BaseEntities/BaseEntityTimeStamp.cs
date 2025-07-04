namespace Domain.Entities.BaseEntities;

public abstract class BaseEntityTimeStamp : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
