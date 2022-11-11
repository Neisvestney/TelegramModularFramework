---
title: Introduction
uid: Guides.Introduction
---

# Introduction

[Nuget](https://www.nuget.org/packages/TelegramModularFramework/) package `TelegramModularFramework`

## Quick start

@Guides.GettingStarted.Installation

## Services

Framework uses [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)  

```csharp
var host = Host.CreateDefaultBuilder(args); 

<services init>

await host.RunAsync();
```

Asp Net host can be used too with `builder.Host`  
To add framework to host configure it with:

- @TelegramModularFramework.TelegramBotHostBuilderExtensions.ConfigureTelegramBotHost  
  Used to add basics services add start recieving updates and routing events with
  @TelegramModularFramework.Services.TelegramBotEvents

```csharp
host.ConfigureTelegramBotHost((context, c) =>
{
    c.TelegramBotClientOptions = new TelegramBotClientOptions(context.Configuration["Telegram:Token"]);
})
```

- @TelegramModularFramework.TelegramBotHostBuilderExtensions.AddTelegramModulesService  
  Used to add [Modules](xref:Guides.TelegramModule) and process updates

```csharp
host.AddTelegramModulesService((context, c) => {})
```

To add modules, set bot commands, handle post execution you need to create Hosted Service
```csharp
public class TelegramHandler: BackgroundService
{
    private readonly TelegramBotEvents _events;
    private readonly ILogger<TelegramHandler> _logger;
    private readonly TelegramModulesService _modulesService;

    public TelegramHandler(TelegramBotEvents events, ILogger<TelegramHandler> logger, TelegramModulesService modulesService)
    {
        _events = events;
        _logger = logger;
        _modulesService = modulesService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _events.OnUpdate += _modulesService.HandleUpdateAsync;
        _modulesService.AddModules();
        await _modulesService.SetMyCommands();
    }
}
```
```csharp
host.ConfigureServices(services =>
{
    services.AddHostedService<TelegramHandler>();
});
```
For more examples see @Guides.GettingStarted.SimpleBot guide and samples

## Samples

- [Commands, actions, stages and callbacks](https://github.com/Neisvestney/TelegramModularFramework/tree/master/samples/TelegramModularFramework.Sample)