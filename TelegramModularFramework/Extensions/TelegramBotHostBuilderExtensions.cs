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

/// <summary>
/// Extends <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> with TelegramModularFramework configuration methods.
/// </summary>
public static class TelegramBotHostBuilderExtensions
{
    /// <summary>
    /// Adds and configures a <see cref="T:Telegram.Bot.ITelegramBotClient"/> along with the required services.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate to configure <see cref="T:TelegramModularFramework.Services.TelegramBotHostConfiguration"/></param>
    /// <returns>Host builder</returns>
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
    
    /// <summary>
    /// Adds and configures a <see cref="T:TelegramModularFramework.Services.TelegramModulesService"/> along with the required services and basics <see cref="T:TelegramModularFramework.Services.TypeReaders.ITypeReader">TypeReaders</see>.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate to configure <see cref="T:TelegramModularFramework.Services.TelegramModulesConfiguration"/></param>
    /// <returns>Host builder</returns>
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
            services.AddSingleton<ITypeReader, BooleanTypeReader>();
            services.AddSingleton<ITypeReader, DoubleTypeReader>();
            services.AddSingleton<ITypeReader, FloatTypeReader>();
        });
    }
}