namespace AiPlayground.Api.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflows(this IServiceCollection services)
        {
            services
                .AddScoped<ActionWorkflow>();

            return services;
        }
    }
}
