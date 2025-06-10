using System.Text.Json.Serialization;

namespace AiPlayground.Api.Models.Actions
{
    public class ActionInputParameterModel
    {
        [JsonPropertyName("example_value")]
        public required string ExampleValue { get; set; }

        [JsonPropertyName("is_required")]
        public required bool IsRequired { get; set; }

        [JsonPropertyName("parameter_name")]
        public required string ParameterName { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        public override string ToString()
        {
            return $"{ParameterName} as {Type}";
        }
    }
}
