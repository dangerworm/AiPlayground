namespace AiPlayground.Api.Models.Playground
{
    public class PlaygroundSetupResponseModel
    {
        public required IEnumerable<string> AvailableModels { get; set; }
        public required int CellSize { get; set; }
        public required int GridWidth { get; set; }
        public required int GridHeight { get; set; }
    }
}
