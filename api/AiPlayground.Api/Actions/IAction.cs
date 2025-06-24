
namespace AiPlayground.Api.Actions;

public interface IAction
{
    public Task<string?> Run(Guid characterId);
}
