namespace AiPlayground.Data.Repositories;

public class ModelRepository
{
    public IEnumerable<string> GetModels()
    {
        return Constants.AvailableModels;
    }
}
