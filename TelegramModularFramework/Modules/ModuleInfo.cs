using Microsoft.Extensions.DependencyInjection;

namespace TelegramModularFramework.Modules;

public class ModuleInfo
{
    public Type Type { get; set; }
    public ObjectFactory Factory { get; set; }
}