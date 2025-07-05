using System.Text.Json.Serialization;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Api.ViewModels;

public class QuestionAnswerViewModel(QuestionAnswerModel questionAnswerModel)
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = questionAnswerModel.Id;

    [JsonPropertyName("character_id")]
    public Guid CharacterId { get; set; } = questionAnswerModel.CharacterId;

    [JsonPropertyName("question")]
    public string Question { get; set; } = questionAnswerModel.Question;

    [JsonPropertyName("answer")]
    public string? Answer { get; set; } = questionAnswerModel.Answer;

    public QuestionAnswerModel ToModel()
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
