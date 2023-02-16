---
title: SimpleBot
uid: Guides.GettingStarted.SimpleBot
---

# Simple Bot

Simple bot with one command  
Make sure that you [installed all required dependencies](xref:Guides.GettingStarted.Installation) and created project with 
[Worker Service Template](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers)

## Code

`Program.cs`
```csharp
using Telegram.Bot;
using TelegramModularFramework;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureTelegramBotHost((context, c) =>
    {
        c.TelegramBotClientOptions = new TelegramBotClientOptions(context.Configuration["Telegram:Token"]);
    })
    .AddTelegramModulesService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<TelegramHandler>();
    })
    .Build();

await host.RunAsync();
```

`TelegramHandler.cs`
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
        _events.OnUpdate += HandleUpdateAsync;
        _events.OnError += HandleErrorAsync;
        _modulesService.CommandExecuted += OnCommandExecuted;
        
        _modulesService.AddModules();
        await _modulesService.SetMyCommands();
    }
    
    private async Task OnCommandExecuted(CommandInfo? commandInfo, ModuleContext context, Result result)
    {
        if (!result.Success)
        {
            var errorMessage = result.Exception switch
            {
                UnknownCommand unknownCommand => context.Update.Message.Chat.Type == ChatType.Private 
                    ? $"Unknown command **{context.CommandString}**" 
                    : null,
                BadArgs badArgs => $"Too few arguments",
                TypeConvertException typeConvert => $"{typeConvert.ErrorReason} at position {typeConvert.Position + 1}",
                BaseCommandException => result.Exception.Message,
                _ => null
            };
            if (errorMessage != null)
            {
                await context.Client.SendTextMessageAsync(context.Update.Message.Chat.Id, errorMessage, parseMode: ParseMode.MarkdownV2);
            }
        }
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!await _modulesService.HandleUpdateAsync(botClient, update, cancellationToken))
        {
            // Handle other update types
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        
    }
}
```

`Module.cs`
```csharp
public class Module: TelegramModule
{
    [Command]
    public async Task Start()
    {
        await ReplyAsync($"Welcome!");
    }
}
```

Then add telegram api key with `dotnet user-secrets set Telegram:Token "Token"`  
And run your app