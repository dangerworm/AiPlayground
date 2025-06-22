using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Actions
{
    public class AskAction : ActionBase, IAction
    {
        public AskAction()
        {
            Description = "Ask a question to the system for further information.";
        }

        [JsonPropertyName("question")]
        [ExampleValue("Why am I here?")]
        public required string Question { get; set; }

        public string Run()
        {
            // In a real implementation, this method would process the question and return a response.
            // For now, we will just return a placeholder response.
            return @$"You asked ""{Question}"". This function is under development and so this is just a placeholder response.";
        }
    }
}
