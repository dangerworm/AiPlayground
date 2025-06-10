using System.Text;
using AiPlayground.Api.Workflows;

namespace AiPlayground.Api.Services
{
    public class PromptService
    {
        private const string SystemPromptFileName = "SystemPrompt.txt";

        private readonly ActionWorkflow _actionWorkflow;

        public PromptService(ActionWorkflow actionWorkflow)
        {
            _actionWorkflow = actionWorkflow ?? throw new ArgumentNullException(nameof(actionWorkflow));
        }

        public string GetSystemPrompt()
        {
            var builder = new StringBuilder();

            var systemPrompt = ReadConfigFile(SystemPromptFileName);
            var actions = _actionWorkflow.GetAvailableActions();

            builder.AppendLine(systemPrompt);
            builder.AppendLine(actions);

            return builder.ToString();
        }

        private string ReadConfigFile(string fileName)
        {
            var systemPromptPath = Path.Combine(AppContext.BaseDirectory, "Config", fileName);
            if (!File.Exists(systemPromptPath))
            {
                throw new FileNotFoundException("System prompt file not found.", systemPromptPath);
            }
            return File.ReadAllText(systemPromptPath);
        }
    }
}
