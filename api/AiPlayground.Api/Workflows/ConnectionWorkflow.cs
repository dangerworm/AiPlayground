using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class ConnectionWorkflow
(
    ILogger<ConnectionWorkflow> logger,
    ModelRepository modelRepository
) : WorkflowBase(logger)
{
    private readonly ModelRepository _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
    
    public ConnectionDto CreateConnection(string modelName)
    {
        var availablemodels = _modelRepository.GetModels();

        if (!availablemodels.Contains(modelName))
        {
            throw new ArgumentException($"Model '{modelName}' is not available.", nameof(modelName));
        }

        return new ConnectionDto
        (
            id: Guid.NewGuid(),
            createdAt: DateTime.UtcNow,
            endpoint: "api/chat",
            host: "localhost",
            model: modelName,
            port: 11434,
            temperature: 0.7m
        );
    }
}
