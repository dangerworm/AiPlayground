using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class CharacterEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required ConnectionEntity Connection { get; set; }
    public required int AgeInEnvironmentIterations { get; set; }
    public required string Colour { get; set; }
    public required Tuple<int, int> GridPosition { get; set; } = new Tuple<int, int>(0, 0);

    public CharacterDto AsDto()
    {
        return new CharacterDto(Id, CreatedAt, Connection.AsDto(), AgeInEnvironmentIterations, Colour, GridPosition);
    }
}
