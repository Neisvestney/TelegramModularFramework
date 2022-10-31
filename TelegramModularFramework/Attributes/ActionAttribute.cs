namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute: Attribute
{
    public string Name { get; }

    public ActionAttribute(string name = null)
    {
        Name = name;
    }
}