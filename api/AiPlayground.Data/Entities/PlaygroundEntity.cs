using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class PlaygroundEntity
{
    public int Iterations { get; set; }
    public IEnumerable<ItemEntity>? Items { get; set; } = [];

    public PlaygroundEntity()
    {
        Iterations = 0;
    }

    public PlaygroundDto AsDto()
    {
        return new PlaygroundDto
        {
            Iterations = Iterations,
            Items = Items?.Select(item => item.AsDto())
        };
    }
}
