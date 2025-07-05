using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Data.Entities;

public class CharacterEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required int AgeInIterations { get; set; }
    public required int CreatedInIteration { get; set; }
    public required string Name { get; set; }
    public required string Colour { get; set; }
    public required Tuple<int, int> GridPosition { get; set; } = new Tuple<int, int>(0, 0);
    public required List<EnvironmentInputModel> Inputs { get; set; }
    public required List<CharacterResponseModel> Responses { get; set; }
    public required List<QuestionAnswerModel> QuestionsAndAnswers { get; set; }

    public CharacterDto AsDto()
    {
        return new CharacterDto(
            Id, 
            CreatedAt, 
            AgeInIterations, 
            CreatedInIteration, 
            Name,
            Colour, 
            GridPosition,
            Inputs,
            Responses,
            QuestionsAndAnswers
        );
    }
}
