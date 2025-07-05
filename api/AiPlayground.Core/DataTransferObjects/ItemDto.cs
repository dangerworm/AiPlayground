using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Core.DataTransferObjects;

public class ItemDto
{
    public ItemDto(
        Guid id,
        DateTime createdAt,
        string content,
        int createdInIteration,
        Tuple<int, int> gridPosition
    )
    {
        Id = id;
        CreatedAt = createdAt;
        Content = content;
        CreatedInIteration = createdInIteration;
        GridPosition = gridPosition;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public string Content { get; }
    public int CreatedInIteration { get; }
    public Tuple<int, int> GridPosition { get; }
}