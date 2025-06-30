using AiPlayground.Core.Models.Configuration;

namespace AiPlayground.Api.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflows(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<CharacterWorkflow>()
                .AddScoped<PlaygroundWorkflow>()
                .AddScoped<PromptWorkflow>();

            return services;
        }
    }
}
