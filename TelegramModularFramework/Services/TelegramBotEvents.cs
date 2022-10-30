using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramModularFramework.Services;

public class TelegramBotEvents: IUpdateHandler
{
    public event Func<ITelegramBotClient, Update, CancellationToken, Task> OnUpdate;
    public event Func<ITelegramBotClient, Exception, CancellationToken, Task> OnError;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await OnUpdate?.InvokeAsync(botClient, update, cancellationToken);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        await OnError?.InvokeAsync(botClient, exception, cancellationToken);
    }
}
