using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class EnvironmentEntity
{
    public required int Iterations { get; set; }

    public EnvironmentDto AsDto()
    {
        return new EnvironmentDto(Iterations);
    }
}
