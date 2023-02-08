using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramModularFramework.Services;
using TelegramModularFramework.WebHook.Services;

namespace TelegramModularFramework.WebHook;

public static class TelegramBotWebHookHostBuilderExtensions
{
    public static IHostBuilder ConfigureTelegramBotWebHookHost(this IHostBuilder builder, Action<HostBuilderContext, TelegramBotWebHookHostConfiguration> config)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.Configure<TelegramBotWebHookHostConfiguration>(c => config(context, c));
            services.AddSingleton<ITelegramBotClient, InjectableTelegramBotClient<TelegramBotWebHookHostConfiguration>>();
            services.AddTelegramBotHostBasics();
            services.AddHostedService<TelegramBotWebHookHostedService>();
        });
    }
}