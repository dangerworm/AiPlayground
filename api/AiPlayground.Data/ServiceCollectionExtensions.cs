using AiPlayground.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AiPlayground.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<CharacterRepository>()
            .AddScoped<EnvironmentRepository>()
            .AddScoped<MemoryRepository>()
            .AddScoped<ModelRepository>();

        return services;
    }
}
