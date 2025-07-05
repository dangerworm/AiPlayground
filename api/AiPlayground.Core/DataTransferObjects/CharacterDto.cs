using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;

namespace AiPlayground.Core.DataTransferObjects;

public class CharacterDto
{
    public CharacterDto(
        Guid id,
        DateTime createdAt,
        int ageInIterations,
        int createdInIteration,
        string name,
        string colour,
        Tuple<int, int> gridPosition,
        IEnumerable<EnvironmentInputModel> inputs,
        IEnumerable<CharacterResponseModel> responses,
        IEnumerable<QuestionAnswerModel> questions
    )
    {
        Id = id;
        CreatedAt = createdAt;
        AgeInIterations = ageInIterations;
        CreatedInIteration = createdInIteration;
        Name = name;
        Colour = colour;
        GridPosition = gridPosition;
        Inputs = inputs;
        Responses = responses;
        Questions = questions;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public int AgeInIterations { get; }
    public int CreatedInIteration { get; }
    public string Name { get; }
    public string Colour { get; }
    public Tuple<int, int> GridPosition { get; }
    public IEnumerable<EnvironmentInputModel> Inputs { get; }
    public IEnumerable<CharacterResponseModel> Responses { get; }
    public IEnumerable<QuestionAnswerModel> Questions { get; }
}