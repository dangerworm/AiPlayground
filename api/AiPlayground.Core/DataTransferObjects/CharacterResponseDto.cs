namespace AiPlayground.Core.DataTransferObjects
{
    public class CharacterResponseDto
    {
        public IList<string> Decisions { get; set; } = [];
        public IList<string> Desires { get; set; } = [];
        public string Emotion { get; set; } = string.Empty;
        public string Thoughts { get; set; } = string.Empty;
    }
}
