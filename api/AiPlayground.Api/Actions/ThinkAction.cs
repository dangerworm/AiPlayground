
namespace AiPlayground.Api.Actions;

public class ThinkAction : ActionBase, IAction
{
    public override string Description => "Continue your internal monologue without taking action.";

    public async Task<string?> Run(Guid characterId)
    {
        return null;
    }

    public override void Setup(string decision)
    {
        throw new NotImplementedException();
    }
}
