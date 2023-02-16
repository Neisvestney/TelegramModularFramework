using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Modules;

public class ActionInfo: BaseInfo
{
    public ActionAttribute Attributes { get; set; }
    
    public bool WithArgs { get; set; }
}