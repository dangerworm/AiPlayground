namespace AiPlayground.Api.Actions;

public abstract class ActionBase
{
    public abstract string Description { get; }

    public abstract void Setup(string decision);
}
