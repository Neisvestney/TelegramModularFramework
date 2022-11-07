using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Attributes;


/// <summary>
/// Selects the operating mode of the command, action or state handler
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RunModeAttribute: Attribute
{
    public RunMode RunMode { get; }

    public RunModeAttribute(RunMode runMode)
    {
        RunMode = runMode;
    }
}