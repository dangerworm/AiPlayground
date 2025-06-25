using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class ConnectionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string Endpoint { get; set; }
    public required string Host { get; set; }
    public required string Model { get; set; }
    public required int Port { get; set; }
    public required decimal Temperature { get; set; }

    public ConnectionDto AsDto()
    {
        return new ConnectionDto(Id, CreatedAt, Endpoint, Host, Model, Port, Temperature);
    }
}
