using System.Reflection;
using AiPlayground.Core.Constants;
using AiPlayground.Core.Models.Conversations;
using AiPlayground.Core.Models.Interactions;
using AiPlayground.Data.Entities;

namespace AiPlayground.Data.Repositories;

public class CharacterRepository : JsonFileStore
{
    protected override string FileName => "Characters.json";

    public async Task<CharacterEntity> CreateCharacterAsync(int createdInIteration, string colour, Tuple<int, int> gridPosition)
    {
        var characters = await LoadAsync<List<CharacterEntity>>() ?? [];
        var characterList = characters.ToList();

        var character = new CharacterEntity
        {
            AgeInIterations = 0,
            CreatedInIteration = createdInIteration,
            Colour = colour,
            Name = $"Agent {characterList.Count + 1}",
            GridPosition = gridPosition,
            Inputs = [],
            Responses = [],
            QuestionsAndAnswers = []
        };

        characterList.Add(character);
        await SaveAsync(characterList);

        return character;
    }

    public async Task<IEnumerable<CharacterEntity>> GetCharactersAsync()
    {
        var characters = await LoadAsync<List<CharacterEntity>>();
        return characters ?? [];
    }

    public async Task<CharacterEntity> GetCharacterByIdAsync(Guid id)
    {
        var characters = await LoadAsync<List<CharacterEntity>>();
        return characters?.FirstOrDefault(c => c.Id == id)
               ?? throw new KeyNotFoundException($"Character with ID {id} not found.");
    }

    public async Task<CharacterEntity> AddIterationMessagesAsync(
        Guid characterId,
        EnvironmentInputModel input,
        CharacterResponseModel response
    )
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
               ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        character.Inputs.Add(input);
        character.Responses.Add(response);

        character.AgeInIterations += 1;

        await SaveAsync(characters);

        return character;
    }

    public async Task AddQuestion(Guid characterId, string question)
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
                   ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        var questionAnswerModel = new QuestionAnswerModel
        {
            CharacterId = characterId,
            Question = question
        };

        character.QuestionsAndAnswers.Add(questionAnswerModel);

        await SaveAsync(characters);
    }

    public async Task AddQuestionAnswers(IEnumerable<QuestionAnswerModel> questionAnswerModels)
    {
        var characters = await GetCharactersAsync();

        var characterQuestionGroups = questionAnswerModels
            .GroupBy(qa => qa.CharacterId)
            .ToDictionary(
                characterGroup => characterGroup.Key,
                characterGroup => characterGroup
                    .GroupBy(model => model.Id)
                    .ToDictionary(questionGroup => questionGroup.Key, questionGroup => questionGroup.ToList()));

        foreach (var characterQuestionGroup in characterQuestionGroups)
        {
            var character = characters?.FirstOrDefault(c => c.Id == characterQuestionGroup.Key)
                   ?? throw new KeyNotFoundException($"Character with ID '{characterQuestionGroup.Key}' not found.");

            var questionIds = characterQuestionGroup.Value.Keys;
            var characterQuestionsToUpdate = character.QuestionsAndAnswers.Where(q => questionIds.Contains(q.Id));
           
            foreach(var question in characterQuestionsToUpdate)
            {
                var answer = characterQuestionGroup.Value[question.Id].First().Answer;
                question.Answer = characterQuestionGroup.Value[question.Id].First().Answer;
            }
        }

        await SaveAsync(characters);
    }

    public async Task<CharacterEntity> UpdatePositionByDeltaAsync(Guid characterId, int dx, int dy)
    {
        var characters = await GetCharactersAsync();
        var character = characters?.FirstOrDefault(c => c.Id == characterId)
               ?? throw new KeyNotFoundException($"Character with ID {characterId} not found.");

        var newX = Math.Max(0, Math.Min(PlaygroundConstants.GridWidth - 1, character.GridPosition.Item1 + dx));
        var newY = Math.Max(0, Math.Min(PlaygroundConstants.GridHeight - 1, character.GridPosition.Item2 + dy));

        var otherCharacters = characters.Where(c => c.Id != characterId).ToList();

        if (otherCharacters.Any(other => other.GridPosition.Item1 == newX &&
                                         other.GridPosition.Item2 == newY))
        {
            throw new InvalidOperationException($"Cannot move to ({newX}, {newY}) as another character is already there.");
        }

        character.GridPosition = new Tuple<int, int>(newX, newY);

        await SaveAsync(characters);

        return character;
    }

    public async Task ResetCharactersAsync()
    {
        var characters = await LoadAsync<List<CharacterEntity>>() ?? [];
        characters.Clear();
        await SaveAsync(characters);
    }
}