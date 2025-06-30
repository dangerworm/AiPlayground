namespace AiPlayground.Core.Models.Configuration
{
    public class AzureOpenAiConnectionConfiguration
    {
        public required string DeploymentKey { get; set; }
        public required string DeploymentName { get; set; }
        public required string Endpoint { get; set; }
        public required string Model { get; set; }
    }
}
