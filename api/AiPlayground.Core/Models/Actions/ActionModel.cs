using System.Text.Json.Serialization;

namespace AiPlayground.Core.Models.Actions;

public class ActionModel
{
    [JsonPropertyName("action_name")]
    public required string ActionName { get; set; }
    
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("input_parameters")]
    public IList<ActionInputParameterModel>? InputParameters { get; set; }

    public override string ToString()
    {
        return $"{ActionName}({GetParameterNamesAndTypes()}): {Description}{GetUsageExample()}";
    }

    private string GetParameterNamesAndTypes()
    {
        if (InputParameters == null || !InputParameters.Any())
        {
            return string.Empty;
        }

        var parameters = InputParameters.Select(p => p.ToString());
        return $"{string.Join(", ", parameters)}";
    }

    private string GetUsageExample()
    {
        if (InputParameters == null || !InputParameters.Any())
        {
            return string.Empty;
        }

        var quotedTypes = new List<string>
        {
            nameof(String),
            nameof(DateTime)
        };

        var exampleValues = InputParameters.Select(p => p.ExampleValue);
        return $" e.g. {ActionName}({string.Join(", ", exampleValues)})";
    }
}
