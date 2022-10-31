namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute: Attribute
{
    public string Name { get; }
    public bool HideFromList { get; }
    
    public CommandAttribute() { }
    
    public CommandAttribute(string name = null, bool hideFromList = false)
    {
        Name = name;
        HideFromList = hideFromList;
    }
}