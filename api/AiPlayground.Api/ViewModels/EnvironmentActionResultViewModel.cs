using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.ViewModels;

public class EnvironmentActionResultViewModel(EnvironmentActionResultModel environmentActionResultModel)
{
    [JsonPropertyName("action_name")]
    public string ActionName { get; set; } = environmentActionResultModel.ActionName;

    [JsonPropertyName("action_result")]
    public string ActionResult { get; set; } = environmentActionResultModel.ActionResult;
}
