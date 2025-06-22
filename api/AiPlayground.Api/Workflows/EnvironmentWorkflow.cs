using System.Text.Json;
using AiPlayground.Api.Models.Conversations;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class EnvironmentWorkflow
(
    ILogger<EnvironmentWorkflow> logger,
    CharacterRepository characterRepository,
    EnvironmentRepository environmentRepository
) : WorkflowBase(logger)
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    private readonly EnvironmentRepository _environmentRepository = environmentRepository ?? throw new ArgumentNullException(nameof(environmentRepository));

    public async Task<string> BuildEnvironmentOutput(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var actionResults = new Dictionary<string, string>();
        var environment = await _environmentRepository.GetEnvironmentAsync();
        var sound = new Dictionary<string, EnvironmentSoundModel>();

        var output = new EnvironmentOutputModel
        {
            ActionResults = actionResults,
            Age = character.AgeInEnvironmentIterations,
            GridPosition = $"[{character.GridPosition.Item1},{character.GridPosition.Item2}]",
            Environment = await DescribeEnvironmentForCharacter(characterId),
            Iteration = environment.Iterations,
            Sound = sound
        };

        return JsonSerializer.Serialize(output, JsonOptions);
    }

    public async Task<string> DescribeEnvironmentForCharacter(Guid characterId)
    {
        var characters = await _characterRepository.GetCharactersAsync();
        if (characters is null || !characters.Any())
        {
            return "No characters found in the environment.";
        }

        if (!characters.Any(c => c.Id != characterId))
        {
            return $"No other characters found in the environment.";
        }

        var descriptions = characters.Select(c => $"Character {c.Id} is at position {c.GridPosition}.").ToList();
        return string.Join(Environment.NewLine, descriptions);
    }
}
