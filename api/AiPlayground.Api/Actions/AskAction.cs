using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Enums;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Actions;

public class AskAction(CharacterRepository characterRepository) : ActionBase, IAction
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));

    [IgnorePropertyDuringProcessing]
    public override ActionType Type => ActionType.CharacterBased;

    [IgnorePropertyDuringProcessing]
    public override string Description => "Ask a question to a human for further information.";

    [JsonPropertyName("question")]
    [ExampleValue("Why am I here?")]
    public required string Question { get; set; }


    public async Task<string> PreIteration(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var question = character.QuestionsAndAnswers.FirstOrDefault(q => string.Equals(q.Question, Question));
        if (question is not null && question.Answer is not null)
        {
            return @$"You asked `{Question}` and received the answer: `{question.Answer}`.";
        }

        return string.Empty;
    }

    public async Task PostIteration(Guid characterId)
    {
        await _characterRepository.AddQuestion(characterId, Question);
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\([""'](.*)[""']\)");

        if (match.Success)
        {
            Question = match.Groups[1].Value;
        }
    }
}
