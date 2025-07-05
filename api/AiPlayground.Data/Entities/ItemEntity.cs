using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class ItemEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string Content { get; set; }
    public required int CreatedInIteration { get; set; }
    public required Tuple<int, int> GridPosition { get; set; } = new Tuple<int, int>(0, 0);

    public ItemDto AsDto()
    {
        return new ItemDto(
            Id, 
            CreatedAt, 
            Content,
            CreatedInIteration, 
            GridPosition
        );
    }
}
