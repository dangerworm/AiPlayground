using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions;

public class MoveAction : ActionBase, IAction
{
    public override string Description => "Move in the direction specified by dx and dy where -1 <= dx <= 1 and -1 <= dy <= 1.";

    [JsonPropertyName("dx")]
    [ExampleValue(3)]
    public required int Dx { get; set; }

    [JsonPropertyName("dy")]
    [ExampleValue(4)]
    public required int Dy { get; set; }

    public async Task<string?> Run(Guid characterId)
    {
        throw new NotImplementedException();
    }
}
