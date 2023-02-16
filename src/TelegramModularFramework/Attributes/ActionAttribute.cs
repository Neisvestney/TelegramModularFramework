namespace TelegramModularFramework.Attributes;

/// <summary>
/// Registers method as action handler with specified name.
/// By default name retrieved from method name in format 'Action Name'.
/// Can be used along with <see cref="T:TelegramModularFramework.Attributes.CommandAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute: Attribute
{
    public string? Name { get; }
    
    public ActionAttribute() {}

    public ActionAttribute(string? name = null)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be empty");
        Name = name;
    }
}