namespace AiPlayground.Core.DataTransferObjects;

public class MemoryDto
{
    public MemoryDto(
        Guid id, 
        Guid characterId, 
        DateTime createdAt, 
        string content)
    {
        Id = id;
        CharacterId = characterId;
        CreatedAt = createdAt;
        Content = content;
    }

    public Guid Id { get; }
    public Guid CharacterId { get; }
    public DateTime CreatedAt { get; }
    public string Content { get; }
}
