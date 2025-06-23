using System.Text.Json;

namespace AiPlayground.Data
{
    public abstract class JsonFileStore()
    {
        protected abstract string FilePath { get; }

        public async Task<T> LoadAsync<T>()
            where T : new()
        {
            if (!File.Exists(FilePath))
            {
                await File.Create(FilePath).DisposeAsync();
            }

            var json = await File.ReadAllTextAsync(FilePath);

            try
            {
                var value = JsonSerializer.Deserialize<T>(json);
                return value is null
                ? new()
                : value;
            }
            catch (JsonException ex)
            {
                return new();
            }
        }

        public async Task SaveAsync<T>(T data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
