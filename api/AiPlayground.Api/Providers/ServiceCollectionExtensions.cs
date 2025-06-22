namespace AiPlayground.Api.Providers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services
            .AddScoped<ActionProvider>();

        return services;
    }
}
