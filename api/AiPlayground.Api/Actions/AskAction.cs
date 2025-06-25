using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions;

public class AskAction : ActionBase, IAction
{
    public override string Description => "Ask a question to a human for further information.";

    [JsonPropertyName("question")]
    [ExampleValue("Why am I here?")]
    public required string Question { get; set; }

    public async Task<string?> Run(Guid characterId)
    {
        // In a real implementation, this method would process the question and return a response.
        // For now, we will just return a placeholder response.
        return @$"You asked ""{Question}"". This function is under development and so this is just a placeholder response.";
    }

    public override void Setup(string decision)
    {
        throw new NotImplementedException();
    }
}
