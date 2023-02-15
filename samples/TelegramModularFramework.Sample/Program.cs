using Serilog;
using Serilog.Events;
using Telegram.Bot;
using TelegramModularFramework;
using TelegramModularFramework.Sample;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Debug)
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

try
{
    Log.Information("Starting host");
    
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureTelegramBotHost((context, c) =>
        {
            c.TelegramBotClientOptions = new TelegramBotClientOptions(context.Configuration["Telegram:Token"]);
        })
        .AddTelegramModulesService()
        .ConfigureServices(services =>
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddHostedService<TelegramHandler>();
            services.AddScoped<SampleService>();
            services.AddSingleton<UsersService>();
        })
        .Build();

    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}