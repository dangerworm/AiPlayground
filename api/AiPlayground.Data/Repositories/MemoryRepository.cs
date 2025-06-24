using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class MemoryRepository : JsonFileStore
{
    protected override string FilePath => "Memories.json";

    public async Task<IList<MemoryEntity>> GetMemoriesAsync()
    {
        var memory = await LoadAsync<List<MemoryEntity>>();
        return memory ?? [];
    }
    
    public async Task SaveMemoryAsync(IList<MemoryEntity> memory)
    {
        await SaveAsync(memory);
    }
}
