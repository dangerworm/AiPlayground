using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions;

public class RememberAction : ActionBase, IAction
{
    public override string Description => "Remember a fact.";

    [JsonPropertyName("memory")]
    [ExampleValue("There is a special block at (4,5).")]
    public required string Memory { get; set; }

    public async Task<string> Run(Guid characterId)
    {
        return "The application is still under construction and this function is not implemented yet, " +
               "but don't worry! All of your memories are in your chat history.";
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\(""(.*)""\)");

        if (match.Success)
        {
            Memory = match.Groups[1].Value;
        }
    }
}
