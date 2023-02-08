using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramModularFramework.Services;

/// <summary>
/// Hosted service to start polling from Telegram API
/// <see cref="TelegramModularFramework.Services.TelegramBotEvents"/> receives updates
/// </summary>
public class TelegramBotHostedService: BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotHostedService> _logger;
    private readonly TelegramBotEvents _events;
    private readonly TelegramBotUser _telegramBotUser;

    public TelegramBotHostedService(ITelegramBotClient botClient, ILogger<TelegramBotHostedService> logger, TelegramBotEvents events, TelegramBotUser telegramBotUser)
    {
        _botClient = botClient;
        _logger = logger;
        _events = events;
        _telegramBotUser = telegramBotUser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        var user = await _botClient.GetMeAsync(stoppingToken);
        _telegramBotUser.User = user;
        _logger.LogInformation("Connected as {username} with id {id}", user.Username, user.Id);

        _botClient.StartReceiving(
            updateHandler: _events,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
    }
}