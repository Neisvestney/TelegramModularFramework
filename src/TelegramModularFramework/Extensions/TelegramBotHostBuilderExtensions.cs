using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramModularFramework.Localization;
using TelegramModularFramework.Services;
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
            services.AddSingleton<ITelegramBotClient, InjectableTelegramBotClient<TelegramBotHostConfiguration>>();
            services.AddTelegramBotHostBasics();
            services.AddHostedService<TelegramBotHostedService>();
        });
    }
    
    /// <summary>
    /// Not public API. Used by <see cref="N:TelegramModularFramework.WebHook"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramBotHostBasics(this IServiceCollection services)
    {
        services.AddSingleton<TelegramBotEvents>();
        services.AddSingleton<IUpdateHandler>(s => s.GetService<TelegramBotEvents>());
        services.AddSingleton<TelegramBotUser>();
        return services;
    } 
    
    /// <summary>
    /// Adds and configures a <see cref="T:TelegramModularFramework.Services.TelegramModulesService"/> along with the required services and basics <see cref="T:TelegramModularFramework.Services.TypeReaders.ITypeReader">TypeReaders</see>.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="config">The delegate to configure <see cref="T:TelegramModularFramework.Services.TelegramModulesConfiguration"/></param>
    /// <returns>Host builder</returns>
    public static IHostBuilder AddTelegramModulesService(this IHostBuilder builder, Action<HostBuilderContext, TelegramModulesConfiguration>? config = null)
    {
        return builder.ConfigureServices((context, services) =>
        {
            if (config != null) services.Configure<TelegramModulesConfiguration>(c => config(context, c));
            services.AddTransient(c => c.GetService<IOptions<TelegramModulesConfiguration>>().Value.StateHolder);
            services.AddSingleton<TelegramModulesService>();
            services.AddTransient<IStringSplitter, StringSplitter>();
            services.AddTransient(c => c.GetService<IOptions<TelegramModulesConfiguration>>().Value.CultureInfoUpdater);
            services.AddTransient(c => c.GetService<IOptions<TelegramModulesConfiguration>>().Value.TypeReadersMessagesStringLocalizer);
            services.AddSingleton<ITypeReader, StringTypeReader>();
            services.AddSingleton<ITypeReader, IntTypeReader>();
            services.AddSingleton<ITypeReader, BooleanTypeReader>();
            services.AddSingleton<ITypeReader, DoubleTypeReader>();
            services.AddSingleton<ITypeReader, FloatTypeReader>();
        });
    }
}