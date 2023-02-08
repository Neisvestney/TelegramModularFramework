using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramModularFramework.Services;

/// <summary>
/// The <see cref=" Telegram.Bot.Extensions.Polling.IUpdateHandler"/> to route <see cref="T:Telegram.Bot.ITelegramBotClient"/> update and error events 
/// </summary>
public class TelegramBotEvents: IUpdateHandler
{
    public event Func<ITelegramBotClient, Update, CancellationToken, Task>? OnUpdate;
    public event Func<ITelegramBotClient, Exception, CancellationToken, Task>? OnError;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (OnUpdate != null) await OnUpdate.InvokeAsync(botClient, update, cancellationToken);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (OnError != null) await OnError.InvokeAsync(botClient, exception, cancellationToken);
    }
}
