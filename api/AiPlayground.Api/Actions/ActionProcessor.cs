using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Conversations;

namespace AiPlayground.Api.Actions;

public class ActionProcessor(ActionProvider actionProvider)
{
    private readonly ActionProvider _actionProvider = actionProvider ?? throw new ArgumentNullException(nameof(actionProvider));

    public async Task<IEnumerable<EnvironmentActionResultModel>> ProcessActions(CharacterDto character)
    {
        var actions = GetActions();
        var decisions = character.Responses.LastOrDefault()?.Decisions;

        if (decisions is null || !decisions.Any())
        {
            return [];
        }

        var actionResults = new List<EnvironmentActionResultModel>();
        foreach (var decision in decisions)
        {
            var command = decision.Contains('(')
                ? decision[..decision.IndexOf('(')]
                : decision;
            
            var action = actions.FirstOrDefault(a => string.Equals(a.GetType().Name, $"{command}Action"));
            if (action is null)
            {
                continue;
            }
            
            action.Setup(decision);
            var result = await action.Run(character.Id);
            if (result is null)
            {
                continue;
            }

            actionResults.Add(new EnvironmentActionResultModel
            {
                ActionName = command,
                ActionResult = result
            });
        }
        
        return actionResults;
    }

    public IEnumerable<IAction> GetActions()
    {
        return _actionProvider.GetActionInstances();
    }
    
    public string GetActionDescriptions()
    {
        return _actionProvider.GetActionDescriptions();
    }
    
    //public string GetActionHelp(string actionName)
    //{
    //    var actions = GetActions();
    //    var action = actions.FirstOrDefault(a => a.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));
    //    return action?.GetHelp() ?? $"No help available for action '{actionName}'.";
    //}
}
