namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SummaryAttribute: Attribute
{
    public string Summary { get; }
    
    public SummaryAttribute(string summary)
    {
        Summary = summary;
    }
}