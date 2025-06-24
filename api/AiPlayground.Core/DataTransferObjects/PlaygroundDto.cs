namespace AiPlayground.Core.DataTransferObjects;

public class PlaygroundDto
{
    public PlaygroundDto(int iterations)
    {
        Iterations = iterations;
    }

    public int Iterations { get; set; }
}
