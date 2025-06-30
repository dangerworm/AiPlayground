namespace AiPlayground.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<CharacterService>()
            .AddScoped<PlaygroundService>();

        return services;
    }
}
