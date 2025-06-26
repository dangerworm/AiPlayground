namespace AiPlayground.Core.DataTransferObjects;

public class PlaygroundDto
{
    public int Iterations { get; set; }

    public IDictionary<Guid, string> CharacterQuestions { get; set; }

    public PlaygroundDto(int iterations, IDictionary<Guid, string> characterQuestions)
    {
        Iterations = iterations;
        CharacterQuestions = characterQuestions;
    }
}
