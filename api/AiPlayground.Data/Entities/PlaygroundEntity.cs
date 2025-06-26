using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class PlaygroundEntity
{
    public int Iterations { get; set; }

    public IDictionary<Guid, string> CharacterQuestions { get; set; }

    public PlaygroundEntity()
    {
        Iterations = 0;
        CharacterQuestions = new Dictionary<Guid, string>();
    }

    public PlaygroundDto AsDto()
    {
        return new PlaygroundDto(Iterations, CharacterQuestions);
    }
}
