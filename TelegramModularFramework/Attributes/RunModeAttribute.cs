using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RunModeAttribute: Attribute
{
    public RunMode RunMode { get; }

    public RunModeAttribute(RunMode runMode)
    {
        RunMode = runMode;
    }
}