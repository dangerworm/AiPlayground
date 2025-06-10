namespace AiPlayground.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<PromptService>();

        return services;
    }
}
