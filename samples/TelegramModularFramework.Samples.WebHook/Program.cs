using Telegram.Bot;
using TelegramModularFramework;
using TelegramModularFramework.Samples.WebHook.Services;
using TelegramModularFramework.WebHook;
using TelegramModularFramework.WebHook.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureTelegramBotWebHookHost((context, c) =>
{
    c.TelegramBotClientOptions = new TelegramBotClientOptions(context.Configuration["Telegram:Token"] ?? throw new InvalidOperationException("Token must be non null"));
    c.SecretToken = context.Configuration["Telegram:WebHook:SecretToken"] ?? throw new InvalidOperationException("Secret must be non null");
}).AddTelegramModulesService((context, c) => {});

builder.Services.Configure<TelegramBotWebHookHostConfiguration>(builder.Configuration.GetSection("WebHookConfiguration"));

builder.Services.AddHostedService<TelegramHandler>();

var app = builder.Build();

app.MapTelegramWebHook();
app.MapGet("/", () => "Hello world");

app.Run();
