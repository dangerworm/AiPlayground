using AiPlayground.Core.Models.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace AiPlayground.Api.OpenAI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureOpenAiChatClient(this IServiceCollection services, IConfiguration configuration)
    {
        var azureOpenAiConfig = configuration.GetSection("AzureOpenAiConnectionConfiguration").Get<AzureOpenAiConnectionConfiguration>();
        if (azureOpenAiConfig is null)
        {
            throw new ArgumentNullException(nameof(azureOpenAiConfig), "Azure OpenAI connection configuration variables are not set.");
        }

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<AzureOpenAiConnectionConfiguration>>().Value;
            var client = new AzureOpenAIClient(
                new Uri(azureOpenAiConfig.Endpoint),
                new AzureKeyCredential(azureOpenAiConfig.DeploymentKey)
            );

            return client.GetChatClient(azureOpenAiConfig.DeploymentName);
        });

        return services;
    }
}
