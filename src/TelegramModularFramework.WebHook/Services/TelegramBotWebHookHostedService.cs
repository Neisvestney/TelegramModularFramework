using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Services;

namespace TelegramModularFramework.WebHook.Services;

public class TelegramBotWebHookHostedService : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotWebHookHostedService> _logger;
    private readonly TelegramBotUser _telegramBotUser;
    private readonly TelegramBotWebHookHostConfiguration _options;

    public TelegramBotWebHookHostedService(ITelegramBotClient botClient,
        ILogger<TelegramBotWebHookHostedService> logger, TelegramBotUser telegramBotUser, IOptions<TelegramBotWebHookHostConfiguration> options)
    {
        _botClient = botClient;
        _logger = logger;
        _telegramBotUser = telegramBotUser;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var user = await _botClient.GetMeAsync(cancellationToken);
        _telegramBotUser.User = user;
        _logger.LogInformation("Connected as {username} with id {id}", user.Username, user.Id);

        await _botClient.SetWebhookAsync(
            url: $"{_options.HostAddress}{_options.Route}",
            certificate: _options.Certificate,
            ipAddress: _options.IpAddress,
            allowedUpdates: _options.AllowedUpdates,
            secretToken: _options.SecretToken,
            dropPendingUpdates: _options.DropPendingUpdates,
            cancellationToken: cancellationToken
        );
        
        _logger.LogInformation("Webhook has been set");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting WebHook");
        await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}