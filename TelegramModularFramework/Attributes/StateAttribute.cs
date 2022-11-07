namespace TelegramModularFramework.Attributes;

/// <summary>
/// Specifies that <see cref="T:TelegramModularFramework.Modules.BaseTelegramModule"/> is state
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StateAttribute: Attribute
{
    public string Name { get; set; }

    public StateAttribute(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be empty");
        if (name.Contains("/")) throw new ArgumentException("Name cannot contain '/'");
        Name = name;
    }
}