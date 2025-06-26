namespace AiPlayground.Core.Constants
{
    public class PlaygroundConstants
    {
        public static IEnumerable<string> AvailableModels =
        [
            "llama3.1:8b",
            "mistral",
        ];

        public const int DefaultCellSizePixels = 20;
        public const int DefaultGridSize = 10;
    }
}
