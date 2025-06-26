using AiPlayground.Core.Constants;

namespace AiPlayground.Data.Repositories;

public class ModelRepository
{
    public IEnumerable<string> GetModels()
    {
        return PlaygroundConstants.AvailableModels;
    }
}
