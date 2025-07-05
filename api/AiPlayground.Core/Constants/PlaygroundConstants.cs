namespace AiPlayground.Core.Constants
{
    public static class PlaygroundConstants
    {
        public const string Gpt41 = "gpt-4.1";
        public const int GridHeight = 10;
        public const int GridWidth = 4;

        public readonly static IEnumerable<string> AvailableModels =
        [
            Gpt41,
        ];
    }
}
