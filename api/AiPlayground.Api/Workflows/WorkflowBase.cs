using System.Text.Json;

namespace AiPlayground.Api.Workflows
{
    public class WorkflowBase(ILogger<WorkflowBase> logger)
    {
        protected ILogger<WorkflowBase> Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        protected JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false
        };
    }
}
