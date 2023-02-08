---
title: WebHook
uid: Guides.WebHook
---

# WebHook

Framework has [WebHook](https://core.telegram.org/bots/webhooks) support for receiving updates
via [TelegramModularFramework.WebHook](https://www.nuget.org/packages/TelegramModularFramework.WebHook/) package and ASP.Net.  
For WebHook example project see [This Sample](https://github.com/Neisvestney/TelegramModularFramework/tree/master/samples/TelegramModularFramework.Samples.WebHook)

## Minimal Project

`Program.cs`
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureTelegramBotWebHookHost((context, c) =>
{
    c.TelegramBotClientOptions = new TelegramBotClientOptions(context.Configuration["Telegram:Token"] ?? throw new InvalidOperationException("Token must be non null"));
    c.SecretToken = context.Configuration["Telegram:WebHook:SecretToken"] ?? throw new InvalidOperationException("Secret must be non null");
}).AddTelegramModulesService();

builder.Services.Configure<TelegramBotWebHookHostConfiguration>(builder.Configuration.GetSection("WebHookConfiguration"));

builder.Services.AddHostedService<TelegramHandler>();

var app = builder.Build();

app.MapTelegramWebHook();

app.Run();
```

For `TelegramHandler` and [Modules](xref:Guides.TelegramModule) samples go to [This Guide](xref:Guides.GettingStarted.SimpleBot)
