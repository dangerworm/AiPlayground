using System.Text.Json;
using AiPlayground.Api.Models.Conversations;
using AiPlayground.Data.Repositories;

namespace AiPlayground.Api.Workflows;

public class CharacterEnvironmentWorkflow
(
    ILogger<PlaygroundWorkflow> logger,
    CharacterRepository characterRepository,
    PlaygroundRepository playgroundRepository
) : WorkflowBase(logger)
{
    private readonly CharacterRepository _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    private readonly PlaygroundRepository _playgroundRepository = playgroundRepository ?? throw new ArgumentNullException(nameof(playgroundRepository));

    public async Task<string> BuildEnvironmentOutputAsync(Guid characterId)
    {
        var character = await _characterRepository.GetCharacterByIdAsync(characterId);
        var actionResults = new Dictionary<string, string>();
        var environment = await _playgroundRepository.GetPlaygroundAsync();
        var sound = new Dictionary<string, EnvironmentSoundModel>();

        var output = new CharacterEnvironmentOutputModel
        {
            ActionResults = actionResults,
            Age = character.AgeInIterations,
            GridPosition = $"[{character.GridPosition.Item1},{character.GridPosition.Item2}]",
            Environment = await DescribeCharacterEnvironmentAsync(characterId),
            Iteration = environment.Iterations,
            Sound = sound
        };

        return JsonSerializer.Serialize(output, JsonOptions);
    }

    public async Task<string> DescribeCharacterEnvironmentAsync(Guid characterId)
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
