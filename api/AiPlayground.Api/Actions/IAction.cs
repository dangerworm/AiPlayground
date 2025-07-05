
using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public interface IAction
{
    public ActionType Type { get; }

    public Task<string> PreIteration(Guid characterId);
    
    public Task PostIteration(Guid characterId);

    public abstract void Setup(string decision);
}
