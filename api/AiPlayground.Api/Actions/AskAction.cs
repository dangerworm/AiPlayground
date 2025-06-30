using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public class AskAction : ActionBase, IAction
{
    public override ActionType ActionType => ActionType.CharacterBased;
    public override string Description => "Ask a question to a human for further information.";

    [JsonPropertyName("question")]
    [ExampleValue("Why am I here?")]
    public required string Question { get; set; }

    public async Task<string> Run(Guid characterId)
    {
        // In a real implementation, this method would process the question and return a response.
        // For now, we will just return a placeholder response.
        return @$"You asked `{Question}`.";
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\([""'](.*)[""']\)");

        if (match.Success)
        {
            Question = match.Groups[1].Value;
        }
    }
}
