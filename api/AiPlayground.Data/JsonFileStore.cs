using System.Text.Json;

namespace AiPlayground.Data
{
    public abstract class JsonFileStore()
    {
        protected abstract string FileName { get; }

        public async Task<T> LoadAsync<T>()
            where T : new()
        {
            var path = GetFilePath();

            if (!File.Exists(path))
            {
                await File.Create(path).DisposeAsync();
            }

            var json = await File.ReadAllTextAsync(path);

            try
            {
                var value = JsonSerializer.Deserialize<T>(json);
                return value is null
                ? new()
                : value;
            }
            catch (JsonException)
            {
                return new();
            }
        }

        public async Task SaveAsync<T>(T data)
        {
            var path = GetFilePath();

            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json);
        }

        private string GetFilePath()
        {
            var targetFolder = "Data";
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            return Path.Combine(targetFolder, FileName);
        }
    }
}
