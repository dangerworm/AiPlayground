using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public abstract class ActionBase
{
    public abstract ActionType Type { get; }

    public abstract string Description { get; }

    public abstract void Setup(string decision);
}
