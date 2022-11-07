using Telegram.Bot;

namespace TelegramModularFramework.Services;

public class TelegramBotHostConfiguration
{
    public TelegramBotClientOptions TelegramBotClientOptions { get; set; }
    
    /// <summary>
    /// HttpClient for <see cref="T:Telegram.Bot.ITelegramBotClient"/>
    /// </summary>
    public HttpClient HttpClient { get; set; }
}