using System.Text.Json.Serialization;
using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class ConnectionEntity
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("endpoint")]
    public required string Endpoint { get; set; }
    
    [JsonPropertyName("host")]
    public required string Host { get; set; }
    
    [JsonPropertyName("model")]
    public required string Model { get; set; }
    
    [JsonPropertyName("port")]
    public required int Port { get; set; }

    [JsonPropertyName("temperature")]
    public required decimal Temperature { get; set; }

    public ConnectionDto AsDto()
    {
        return new ConnectionDto(Id, CreatedAt, Endpoint, Host, Model, Port, Temperature);
    }
}
