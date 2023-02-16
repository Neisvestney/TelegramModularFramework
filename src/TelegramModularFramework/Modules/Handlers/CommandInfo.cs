using System.Reflection;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Modules;

public class CommandInfo: HandlerInfoBase
{
    public CommandAttribute Attributes { get; set; }
    public bool HiddenFromList { get; set; }
}