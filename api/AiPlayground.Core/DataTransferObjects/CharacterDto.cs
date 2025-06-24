namespace AiPlayground.Core.DataTransferObjects;

public class CharacterDto
{
    public CharacterDto(
        Guid id,
        DateTime createdAt,
        ConnectionDto connection,
        int ageInIterations,
        int createdInIteration,
        string colour,
        Tuple<int, int> gridPosition,
        IList<CharacterResponseDto> responses,
        IList<string> questions
    )
    {
        Id = id;
        CreatedAt = createdAt;
        Connection = connection;
        AgeInIterations = ageInIterations;
        CreatedInIteration = createdInIteration;
        Colour = colour;
        GridPosition = gridPosition;
        Responses = responses;
        Questions = questions;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public ConnectionDto Connection { get; }
    public int AgeInIterations { get; }
    public int CreatedInIteration { get; }
    public string Colour { get; }
    public Tuple<int, int> GridPosition { get; }
    public IList<CharacterResponseDto> Responses { get; }
    public IList<string> Questions { get; }
}