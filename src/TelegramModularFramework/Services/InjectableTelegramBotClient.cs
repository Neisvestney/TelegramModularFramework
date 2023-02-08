using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace TelegramModularFramework.Services;

public class InjectableTelegramBotClient<TOptions>: TelegramBotClient where TOptions : TelegramBotHostConfiguration
{
    public InjectableTelegramBotClient(IOptions<TOptions> options): base(options.Value.TelegramBotClientOptions, options.Value.HttpClient)
    {
        
    } 
}