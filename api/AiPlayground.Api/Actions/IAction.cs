
namespace AiPlayground.Api.Actions;

public interface IAction
{
    public Task<string> Run(Guid characterId);

    public abstract void Setup(string decision);
}
