namespace AiPlayground.Core.DataTransferObjects;

public class CharacterDto
{
    public CharacterDto(
        Guid id,
        DateTime createdAt,
        ConnectionDto connection,
        int ageInEnvironmentIterations,
        Tuple<int, int> gridPosition
    )
    {
        Id = id;
        CreatedAt = createdAt;
        Connection = connection;
        AgeInEnvironmentIterations = ageInEnvironmentIterations;
        GridPosition = gridPosition;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public ConnectionDto Connection { get; }
    public int AgeInEnvironmentIterations { get; }
    public Tuple<int, int> GridPosition { get; }
}