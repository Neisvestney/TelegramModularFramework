using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramModularFramework.Services;

public class TelegramBotHostConfiguration
{
    public TelegramBotClientOptions TelegramBotClientOptions { get; set; }
    
    /// <summary>
    /// HttpClient for <see cref="T:Telegram.Bot.ITelegramBotClient"/>
    /// </summary>
    public HttpClient HttpClient { get; set; }

    public IEnumerable<UpdateType> AllowedUpdates { get; set; } = Array.Empty<UpdateType>();
    
    public bool? DropPendingUpdates { get; set; } = default;
}