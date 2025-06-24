using AiPlayground.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AiPlayground.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<CharacterRepository>()
            .AddScoped<MemoryRepository>()
            .AddScoped<ModelRepository>()
            .AddScoped<PlaygroundRepository>();

        return services;
    }
}
