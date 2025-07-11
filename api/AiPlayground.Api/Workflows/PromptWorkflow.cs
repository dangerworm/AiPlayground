﻿using System.Text;
using AiPlayground.Api.Actions;

namespace AiPlayground.Api.Workflows;

public class PromptWorkflow(
    ILogger<PromptWorkflow> logger,
    ActionProvider actionWorkflow
) : WorkflowBase(logger)
{
    private const string SystemPromptFileName = "SystemPrompt.txt";

    private readonly ActionProvider _actionWorkflow = actionWorkflow ?? throw new ArgumentNullException(nameof(actionWorkflow));

    public string GetSystemPrompt()
    {
        var builder = new StringBuilder();

        var systemPrompt = ReadConfigFile(SystemPromptFileName);
        var actions = _actionWorkflow.GetActionDescriptions();

        builder.AppendLine(systemPrompt);
        builder.AppendLine(actions);

        return builder.ToString();
    }

    private string ReadConfigFile(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Config", fileName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found.", path);
        }
        return File.ReadAllText(path);
    }
}
