namespace AiPlayground.Core.Models.Interactions;
public class QuestionAnswerModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid CharacterId { get; init; }
    public required string Question { get; set; }
    public string? Answer { get; set; }
}
