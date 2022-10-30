using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramModularFramework.Services;

public class TelegramBotHostedService: BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotHostedService> _logger;
    private readonly TelegramBotEvents _events;

    public User User => _user;
    private User _user;

    public TelegramBotHostedService(ITelegramBotClient botClient, ILogger<TelegramBotHostedService> logger, TelegramBotEvents events)
    {
        _botClient = botClient;
        _logger = logger;
        _events = events;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        _user = await _botClient.GetMeAsync();
        _logger.LogInformation("Connected as {username} with id {id}", _user.Username, _user.Id);

        _botClient.StartReceiving(
            updateHandler: _events,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
    }
}