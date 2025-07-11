﻿using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Enums;

namespace AiPlayground.Api.Actions;

public class RecallAction : ActionBase, IAction
{
    [IgnorePropertyDuringProcessing]
    public override ActionType Type => ActionType.CharacterBased;

    [IgnorePropertyDuringProcessing]
    public override string Description => "Recall a memory from your vector database.";

    [JsonPropertyName("query")]
    [ExampleValue("What did the message at (0,1) say?")]
    public required string Query { get; set; }

    public async Task<string> PreIteration(Guid characterId)
    {
        return "The application is still under construction and this function is not implemented yet, " +
               "but don't worry! All of your memories are in your chat history.";
    }

    public async Task PostIteration(Guid characterId)
    {
    }

    public override void Setup(string decision)
    {
        var match = Regex.Match(decision, @$"{GetType().Name[..^6]}\([""'](.*)[""']\)");

        if (match.Success)
        {
            Query = match.Groups[1].Value;
        }
    }
}
