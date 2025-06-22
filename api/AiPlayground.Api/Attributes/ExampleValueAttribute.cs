namespace AiPlayground.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExampleValueAttribute : Attribute
    {
        public ExampleValueAttribute(object exampleValue)
        {
            ExampleValue = $"{exampleValue}";
        }

        public string ExampleValue { get; }
    }
}
