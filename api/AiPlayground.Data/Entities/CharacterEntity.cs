using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Data.Entities;

public class CharacterEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required int AgeInIterations { get; set; }
    public required int CreatedInIteration { get; set; }
    public required string Colour { get; set; }
    public required Tuple<int, int> GridPosition { get; set; } = new Tuple<int, int>(0, 0);
    public required IList<EnvironmentInputModel> Inputs { get; set; }
    public required IList<CharacterResponseModel> Responses { get; set; }
    public required IList<string> Questions { get; set; }

    public CharacterDto AsDto()
    {
        return new CharacterDto(
            Id, 
            CreatedAt, 
            AgeInIterations, 
            CreatedInIteration, 
            Colour, 
            GridPosition,
            Inputs,
            Responses,
            Questions
        );
    }
}
