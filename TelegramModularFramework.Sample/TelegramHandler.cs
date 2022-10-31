using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample;

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
        _modulesService.ActionExecuted += ModulesServiceOnActionExecuted;
        
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
    
    private async Task ModulesServiceOnActionExecuted(ActionInfo? actionInfo, ModuleContext context, Result result)
    {
        if (!result.Success)
        {
            var errorMessage = result.Exception switch
            {
                UnknownCommand unknownCommand => $"Unknown action **{context.CommandString}**",
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