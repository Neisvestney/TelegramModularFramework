using TelegramModularFramework.Services;

namespace TelegramModularFramework.WebHook.Services;

public class TelegramBotWebHookHostConfiguration : TelegramBotHostConfiguration
{
    public string HostAddress { get; set; }
    public string Route { get; set; } = "/telegram";
}