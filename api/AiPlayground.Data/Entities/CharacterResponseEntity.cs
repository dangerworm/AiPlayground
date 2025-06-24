using AiPlayground.Core.DataTransferObjects;

namespace AiPlayground.Data.Entities;

public class CharacterResponseEntity
{
    public IList<string> Decisions { get; set; } = [];
    public IList<string> Desires { get; set; } = [];
    public string Emotion { get; set; } = string.Empty;
    public string Thoughts { get; set; } = string.Empty;

    public CharacterResponseDto AsDto()
    {
        return new CharacterResponseDto
        {
            Decisions = Decisions,
            Desires = Desires,
            Emotion = Emotion,
            Thoughts = Thoughts
        };
    }
}
