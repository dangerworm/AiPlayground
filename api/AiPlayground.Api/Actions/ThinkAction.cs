using AiPlayground.Api.Attributes;
using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public class ThinkAction : ActionBase, IAction
{
    [IgnorePropertyDuringProcessing]
    public override ActionType Type => ActionType.CharacterBased;

    [IgnorePropertyDuringProcessing]
    public override string Description => "Continue your internal monologue without taking action.";

    public async Task<string> PreIteration(Guid characterId)
    {
        return "You stopped and thought.";
    }

    public async Task PostIteration(Guid characterId)
    {
    }

    public override void Setup(string decision)
    {
    }
}
