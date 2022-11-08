namespace TelegramModularFramework.Attributes;

/// <summary>
/// Specifies that <see cref="T:TelegramModularFramework.Modules.BaseTelegramModule"/> in group
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GroupAttribute: Attribute
{
    public string Name { get; set; }

    public GroupAttribute(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be empty");
        if (name.Contains("/")) throw new ArgumentException("Name cannot contain '/'");
        Name = name;
    }
}