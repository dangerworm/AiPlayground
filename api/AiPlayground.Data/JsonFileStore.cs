using System.Text.Json;

namespace AiPlayground.Data
{
    public abstract class JsonFileStore()
    {
        protected abstract string FilePath { get; }

        public async Task<T> LoadAsync<T>()
        {
            if (!File.Exists(FilePath))
            {
                throw new FileLoadException($"File not found: {FilePath}");
            }

            var json = await File.ReadAllTextAsync(FilePath);
            var value = JsonSerializer.Deserialize<T>(json);

            return value is null
                ? throw new InvalidOperationException($"Failed to deserialize data from {FilePath}.")
                : value;
        }

        public async Task SaveAsync<T>(T data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
