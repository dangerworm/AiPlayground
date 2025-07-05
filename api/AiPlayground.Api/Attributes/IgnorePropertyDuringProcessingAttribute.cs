namespace AiPlayground.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyDuringProcessingAttribute : Attribute
    {
        public IgnorePropertyDuringProcessingAttribute()
        {
        }
    }
}
