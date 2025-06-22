using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class EnvironmentRepository : JsonFileStore
{
    protected override string FilePath => "Environment.json";

    public async Task<EnvironmentEntity> GetEnvironmentAsync()
    {
        var memory = await LoadAsync<EnvironmentEntity>();
        return memory ?? throw new Exception("No environment information found.");
    }

    public async Task<EnvironmentEntity> UpdateEnvironmentIterations()
    {
        var environment = await GetEnvironmentAsync();
        environment.Iterations += 1;

        await SaveAsync(environment);

        return environment;
    }
}
