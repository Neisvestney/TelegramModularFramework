namespace TelegramModularFramework.Attributes;

/// <summary>
/// Add description to command or action info which can be used after in /help or in <see cref="TelegramModularFramework.Services.TelegramModulesService.SetMyCommands"/>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SummaryAttribute: Attribute
{
    public string Summary { get; }
    
    public SummaryAttribute(string summary)
    {
        Summary = summary;
    }
}