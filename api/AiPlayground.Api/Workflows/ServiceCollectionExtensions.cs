namespace AiPlayground.Api.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflows(this IServiceCollection services)
        {
            services
                .AddScoped<CharacterEnvironmentWorkflow>()
                .AddScoped<CharacterWorkflow>()
                .AddScoped<ConnectionWorkflow>()
                .AddScoped<PlaygroundWorkflow>()
                .AddScoped<PromptWorkflow>();

            return services;
        }
    }
}
