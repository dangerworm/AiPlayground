using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class MemoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CharacterId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string Content { get; set; }

    public MemoryDto AsDto()
    {
        return new MemoryDto(Id, CharacterId, CreatedAt, Content);
    }
}
