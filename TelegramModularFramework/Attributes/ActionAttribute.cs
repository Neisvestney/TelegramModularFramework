namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute: Attribute
{
    public string Name { get; }
    
    public ActionAttribute() {}

    public ActionAttribute(string name = null)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be empty");
        Name = name;
    }
}