namespace AiPlayground.Core.DataTransferObjects;

public class EnvironmentDto
{
    public EnvironmentDto(int iterations)
    {
        Iterations = iterations;
    }

    public int Iterations { get; set; }
}
