using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class MemoryEntity
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("character_id")]
    public required Guid CharacterId { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    public MemoryDto AsDto()
    {
        return new MemoryDto(Id, CharacterId, CreatedAt, Content);
    }
}
