namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class StateHandlerAttribute : Attribute
{
    public bool ParseArgs { get; set; }

    public StateHandlerAttribute()
    {
        ParseArgs = false;
    }
    
    public StateHandlerAttribute(bool parseArgs)
    {
        ParseArgs = parseArgs;
    }
}

