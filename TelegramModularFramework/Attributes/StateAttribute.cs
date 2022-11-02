namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StateAttribute: Attribute
{
    public string Name { get; set; }

    public StateAttribute(string name)
    {
        if (name.Contains("/")) throw new ArgumentException("Name cannot contain '/'");
        Name = name;
    }
}