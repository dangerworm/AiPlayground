using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class EnvironmentEntity
{
    public int Iterations { get; set; }

    public EnvironmentEntity()
    {
        Iterations = 0;
    }

    public EnvironmentDto AsDto()
    {
        return new EnvironmentDto(Iterations);
    }
}
