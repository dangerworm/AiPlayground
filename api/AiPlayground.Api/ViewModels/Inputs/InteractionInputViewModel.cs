using System.Text.Json.Serialization;

namespace AiPlayground.Api.ViewModels.Inputs;

public class InteractionInputViewModel
{
    [JsonPropertyName("question_answers")]
    public IEnumerable<QuestionAnswerInputViewModel>? QuestionAnswers { get; set; }
}
