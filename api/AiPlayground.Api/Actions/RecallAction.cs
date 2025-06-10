using System.Text.Json.Serialization;

namespace AiPlayground.Api.Actions
{
    public class RecallAction : ActionBase, IAction
    {
        public RecallAction()
        {
            Description = "Recall a memory from your vector database.";
        }

        [JsonPropertyName("query")]
        [ExampleValue("What did the message at (0,1) say?")]
        public required string Query { get; set; }

        public string Run()
        {
            throw new NotImplementedException();
        }
    }
}
