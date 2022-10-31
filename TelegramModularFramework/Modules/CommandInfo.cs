using System.Reflection;
using TelegramModularFramework.Attributes;

namespace TelegramModularFramework.Modules;

public class CommandInfo: BaseInfo
{
    public CommandAttribute Attributes { get; set; }
    public bool HiddenFromList { get; set; }
}