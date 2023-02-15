using System.Reflection;

namespace TelegramModularFramework.Modules;

public abstract class BaseInfo
{
    public MethodInfo MethodInfo { get; set; }
    public ModuleInfo Module { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public RunMode RunMode { get; set; }
}