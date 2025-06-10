using AiPlayground.Api.Services;
using AiPlayground.Api.Workflows;

namespace AiPlayground.Api
{
    public class Startup
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services
                .AddWorkflows()
                .AddServices();

            services.AddControllers();

            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "AI Playground API",
                    Description = "An API for interacting with AI models in a playground environment."
                });
            }); 
        }
    }
}
