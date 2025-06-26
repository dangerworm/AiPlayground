namespace AiPlayground.Core.Models.Conversations
{
    public class EnvironmentSoundProcessingModel
    {
        public required Guid CharacterId { get; set; }
        public required EnvironmentSoundModel EnvironmentSoundModel { get; set; }
        public required int MinX { get; set; }
        public required int MaxX { get; set; }
        public required int MinY { get; set; }
        public required int MaxY { get; set; }
    }
}
