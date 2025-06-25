namespace AiPlayground.Api.Actions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        var actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAction).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (var type in actionTypes)
        {
            services.AddTransient(type);
        }

        services
            .AddScoped<ActionProcessor>()
            .AddScoped<ActionProvider>();

        return services;
    }
}
