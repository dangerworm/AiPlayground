using AiPlayground.Core.Constants;

namespace AiPlayground.Data.Repositories;

public class ModelRepository
{
    public IList<string> GetModels()
    {
        return PlaygroundConstants.AvailableModels;
    }
}
