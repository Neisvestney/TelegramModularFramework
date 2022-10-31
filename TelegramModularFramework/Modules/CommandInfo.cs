using System.Reflection;
using TelegramModularFramework.Attributes;

namespace TelegramModularFramework.Modules;

public class CommandInfo
{
    public MethodInfo MethodInfo { get; set; }
    public ModuleInfo Module { get; set; }
    public CommandAttribute Attributes { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public bool HiddenFromList { get; set; }
    public RunMode RunMode { get; set; }
}