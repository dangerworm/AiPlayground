using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class ItemRepository : JsonFileStore
{
    protected override string FileName => "Items.json";

    public async Task<ItemEntity> CreateItemAsync(int createdInIteration, Tuple<int, int> gridPosition, string content)
    {
        var items = await LoadAsync<List<ItemEntity>>() ?? [];
        var itemList = items.ToList();

        var item = new ItemEntity
        {
            Content = content,
            CreatedInIteration = createdInIteration,
            GridPosition = gridPosition,
        };

        itemList.Add(item);
        await SaveAsync(itemList);

        return item;
    }

    public async Task<IEnumerable<ItemEntity>> GetItemsAsync()
    {
        var items = await LoadAsync<List<ItemEntity>>();
        return items ?? [];
    }

    public async Task<ItemEntity> GetItemByIdAsync(Guid id)
    {
        var items = await LoadAsync<List<ItemEntity>>();
        return items?.FirstOrDefault(c => c.Id == id)
               ?? throw new KeyNotFoundException($"Item with ID {id} not found.");
    }

    public async Task ResetItemsAsync()
    {
        var items = await LoadAsync<List<ItemEntity>>() ?? [];
        items.Clear();
        await SaveAsync(items);
    }
}