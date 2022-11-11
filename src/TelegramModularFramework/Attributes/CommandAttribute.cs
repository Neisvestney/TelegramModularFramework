namespace TelegramModularFramework.Attributes;

/// <summary>
/// Registers method as command handler with specified name.
/// By default name retrieved from method name in format 'commandname'.
/// Can be used along with <see cref="T:TelegramModularFramework.Attributes.ActionAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute: Attribute
{
    public string Name { get; }
    
    /// <summary>
    /// Hide from /help and <see cref="TelegramModularFramework.Services.TelegramModulesService.SetMyCommands"/>
    /// </summary>
    public bool HideFromList { get; }
    
    public CommandAttribute() { }
    
    public CommandAttribute(string name = null, bool hideFromList = false)
    {
        if (name != null && name.Contains(' ')) throw new ArgumentException("Command name cannot contain spaces");
        Name = name;
        HideFromList = hideFromList;
    }
}