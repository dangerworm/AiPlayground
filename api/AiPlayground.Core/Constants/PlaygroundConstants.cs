namespace AiPlayground.Core.Constants
{
    public static class PlaygroundConstants
    {
        public const int DefaultCellSizePixels = 20;
        public const int DefaultGridSize = 10;
        public const string Gpt41 = "gpt-4.1";

        public static IEnumerable<string> AvailableModels =
        [
            Gpt41,
        ];
    }
}
