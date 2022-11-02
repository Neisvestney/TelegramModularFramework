using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramModularFramework.Services;
using TelegramModularFramework.Services.State;
using TelegramModularFramework.Services.TypeReaders;
using TelegramModularFramework.Services.Utils;

namespace TelegramModularFramework;

public static class TelegramBotHostBuilderExtensions
{
    public static IHostBuilder ConfigureTelegramBotHost(this IHostBuilder builder, Action<HostBuilderContext, TelegramBotHostConfiguration> config)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.Configure<TelegramBotHostConfiguration>(c => config(context, c));
            services.AddSingleton<ITelegramBotClient, InjectableTelegramBotClient>();
            services.AddSingleton<TelegramBotEvents>();
            services.AddSingleton<IUpdateHandler>(s => s.GetService<TelegramBotEvents>());
            services.AddSingleton<TelegramBotHostedService>();
            services.AddHostedService(s => s.GetService<TelegramBotHostedService>());
        });
    }
    
    public static IHostBuilder AddTelegramModulesService(this IHostBuilder builder, Action<HostBuilderContext, TelegramModulesConfiguration> config)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.Configure<TelegramModulesConfiguration>(c => config(context, c));
            services.AddTransient<IStateHolder>(c => c.GetService<IOptions<TelegramModulesConfiguration>>().Value.StateHolder);
            services.AddSingleton<TelegramModulesService>();
            services.AddTransient<IStringSplitter, StringSplitter>();
            services.AddSingleton<ITypeReader, StringTypeReader>();
            services.AddSingleton<ITypeReader, IntTypeReader>();
        });
    }
}