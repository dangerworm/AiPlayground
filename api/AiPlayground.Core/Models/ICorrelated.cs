namespace AiPlayground.Core.Models
{
    public interface ICorrelated
    {
        public Guid? CorrelationId { get; set; }
    }
}
