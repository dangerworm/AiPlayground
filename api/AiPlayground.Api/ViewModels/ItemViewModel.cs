using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Api.ViewModels;

public class ItemViewModel(ItemDto item)
{
    [JsonPropertyName("id")]
    public Guid Id { get; } = item.Id;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; } = item.CreatedAt;

    [JsonPropertyName("content")]
    public string Content { get; } = item.Content;

    [JsonPropertyName("created_in_iteration")]
    public int CreatedInIteration { get; } = item.CreatedInIteration;

    [JsonPropertyName("grid_position")]
    public Tuple<int, int> GridPosition { get; } = item.GridPosition;
}
