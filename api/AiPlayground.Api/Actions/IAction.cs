
using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public interface IAction
{
    public ActionType ActionType { get; }

    public Task<string> Run(Guid characterId);

    public abstract void Setup(string decision);
}
