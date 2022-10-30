namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute: Attribute
{
    public string Name { get; }
    
    public CommandAttribute() { }
    
    public CommandAttribute(string name)
    {
        Name = name;
    }
}