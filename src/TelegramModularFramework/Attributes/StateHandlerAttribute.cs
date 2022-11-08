namespace TelegramModularFramework.Attributes;

/// <summary>
/// Registers method as state handler of current state module.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class StateHandlerAttribute : Attribute
{
    /// <summary>
    /// If args parsed method receives args as command else receives single string argument.
    /// </summary>
    public bool ParseArgs { get; set; }

    public StateHandlerAttribute()
    {
        ParseArgs = false;
    }

    public StateHandlerAttribute(bool parseArgs = false)
    {
        ParseArgs = parseArgs;
    }
}
