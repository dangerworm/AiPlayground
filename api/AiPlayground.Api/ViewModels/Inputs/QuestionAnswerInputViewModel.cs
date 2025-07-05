using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Api.ViewModels.Inputs;

public class QuestionAnswerInputViewModel()
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("character_id")]
    public Guid CharacterId { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = null!;

    [JsonPropertyName("answer")]
    public string? Answer { get; set; }

    public QuestionAnswerModel FromViewModel()
    {
        return new QuestionAnswerModel
        {
            Id = Id,
            CharacterId = CharacterId,
            Question = Question,
            Answer = Answer
        };
    }
}
