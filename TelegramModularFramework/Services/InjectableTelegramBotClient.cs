using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace TelegramModularFramework.Services;

public class InjectableTelegramBotClient: TelegramBotClient
{
    public InjectableTelegramBotClient(IOptions<TelegramBotHostConfiguration> options): base(options.Value.TelegramBotClientOptions, options.Value.HttpClient)
    {
        
    } 
}