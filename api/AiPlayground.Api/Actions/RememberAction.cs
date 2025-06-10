using System.Text.Json.Serialization;

namespace AiPlayground.Api.Actions
{
    public class RememberAction : ActionBase, IAction
    {
        public RememberAction()
        {
            Description = "Remember a fact.";
        }

        [JsonPropertyName("memory")]
        [ExampleValue("There is a special block at (4,5).")]
        public required string Memory { get; set; }

        public string Run()
        {
            throw new NotImplementedException();
        }
    }
}
