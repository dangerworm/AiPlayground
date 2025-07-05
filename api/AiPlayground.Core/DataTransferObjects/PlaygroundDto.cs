namespace AiPlayground.Core.DataTransferObjects;

public class PlaygroundDto
{
    public int Iterations { get; set; }
    public IEnumerable<ItemDto>? Items { get; set; } = [];
}
