using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions;

public class RecallAction : ActionBase, IAction
{
    public override string Description => "Recall a memory from your vector database.";

    [JsonPropertyName("query")]
    [ExampleValue("What did the message at (0,1) say?")]
    public required string Query { get; set; }

    public async Task<string?> Run(Guid characterId)
    {
        throw new NotImplementedException();
    }

    public override void Setup(string decision)
    {
        throw new NotImplementedException();
    }
}
