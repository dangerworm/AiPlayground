using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class PlaygroundRepository : JsonFileStore
{
    protected override string FileName => "Playground.json";

    public async Task<PlaygroundEntity> GetPlaygroundAsync()
    {
        var playground = await LoadAsync<PlaygroundEntity>();
        return playground ?? throw new Exception("No environment information found.");
    }

    public async Task<PlaygroundEntity> UpdateIterationsAsync()
    {
        var playground = await GetPlaygroundAsync();
        playground.Iterations += 1;

        await SaveAsync(playground);

        return playground;
    }

    public async Task<PlaygroundEntity> ResetPlaygroundAsync()
    {
        var playground = new PlaygroundEntity();
        await SaveAsync(playground);
        return playground;
    }
}
