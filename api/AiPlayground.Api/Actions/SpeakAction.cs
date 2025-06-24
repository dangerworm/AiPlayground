using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions;

public class SpeakAction : ActionBase, IAction
{
    public override string Description => "Speak the string aloud, projecting to a radius of 'projection' across the grid.";
    
    [JsonPropertyName("message")]
    [ExampleValue("Hello, nice to meet you.")]
    public required string Message { get; set; }

    [JsonPropertyName("projection")]
    [ExampleValue(2)]
    public required int Projection { get; set; }

    public async Task<string?> Run(Guid characterId)
    {
        throw new NotImplementedException();
    }
}
