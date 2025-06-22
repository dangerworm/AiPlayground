namespace AiPlayground.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddHttpClient();

        services
            .AddScoped<InteractionService>()
            .AddScoped<PlaygroundService>();

        return services;
    }
}
