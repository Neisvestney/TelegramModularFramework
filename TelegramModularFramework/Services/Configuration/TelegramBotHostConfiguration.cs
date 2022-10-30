using Telegram.Bot;

namespace TelegramModularFramework.Services;

public class TelegramBotHostConfiguration
{
    public TelegramBotClientOptions TelegramBotClientOptions { get; set; }
    public HttpClient HttpClient { get; set; }
}