using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramModularFramework.Services;
using TelegramModularFramework.WebHook.Services;

namespace TelegramModularFramework.WebHook;

public static class TelegramBotWebHookHostBuilderExtensions
{
    /// <summary>
    /// Adds and configures a <see cref="T:Telegram.Bot.ITelegramBotClient"/> along with the required services with WebHook support.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate to configure <see cref="T:TelegramModularFramework.WebHook.Services.TelegramBotWebHookHostConfiguration"/></param>
    /// <returns></returns>
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