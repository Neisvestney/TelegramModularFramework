using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Modules;

public class ActionInfo: HandlerInfoBase
{
    public ActionAttribute Attributes { get; set; }
    
    public bool WithArgs { get; set; }
}