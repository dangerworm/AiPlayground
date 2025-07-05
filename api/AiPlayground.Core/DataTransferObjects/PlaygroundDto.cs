namespace AiPlayground.Core.DataTransferObjects;

public class PlaygroundDto
{
    public int Iterations { get; set; }

    public PlaygroundDto(int iterations)
    {
        Iterations = iterations;
    }
}
