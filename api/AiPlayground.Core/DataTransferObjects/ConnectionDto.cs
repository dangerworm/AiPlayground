namespace AiPlayground.Core.DataTransferObjects;

public class ConnectionDto
{
    public ConnectionDto(
        Guid id,
        DateTime createdAt,
        string endpoint,
        string host,
        string model,
        int port,
        decimal temperature
    )
    {
        Id = id;
        CreatedAt = createdAt;
        Endpoint = endpoint;
        Host = host;
        Model = model;
        Port = port;
        Temperature = temperature;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public string Endpoint { get; }
    public string Host { get; }
    public string Model { get; }
    public int Port { get; }
    public decimal Temperature { get; }
}
