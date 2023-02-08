using Telegram.Bot.Types;
using TelegramModularFramework.Services;

namespace TelegramModularFramework.WebHook.Services;

public class TelegramBotWebHookHostConfiguration : TelegramBotHostConfiguration
{
    public string HostAddress { get; set; }
    public string Route { get; set; } = "/telegram";
    public string SecretToken { get; set; }

    public InputFile? Certificate { get; set; } = default;

    public string? IpAddress { get; set; } = default;

    public int? MaxConnections { get; set; } = default;
}