using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Localization;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample;

public class TelegramHandler: BackgroundService
{
    private readonly TelegramBotEvents _events;
    private readonly ILogger<TelegramHandler> _logger;
    private readonly TelegramModulesService _modulesService;
    private readonly IStringLocalizer<DefaultErrorMessages> _l;

    public TelegramHandler(TelegramBotEvents events, ILogger<TelegramHandler> logger, TelegramModulesService modulesService, IStringLocalizer<DefaultErrorMessages> l)
    {
        _events = events;
        _logger = logger;
        _modulesService = modulesService;
        _l = l;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _events.OnUpdate += HandleUpdateAsync;
        _events.OnError += HandleErrorAsync;
        _modulesService.CommandExecuted += OnCommandExecuted;
        _modulesService.ActionExecuted += ModulesServiceOnActionExecuted;
        _modulesService.StateExecuted += OnStateExecuted;
        _modulesService.CallbackExecuted += OnCallbackExecuted;
        
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
                    ? _l["UnknownCommand", context.CommandString]
                    : null,
                BadArgs badArgs => _l["TooFewArguments"],
                TypeConvertException typeConvert => _l["TypeConvertException", typeConvert.ErrorReason, typeConvert.Position + 1],
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
                UnknownCommand unknownCommand =>  _l["UnknownAction", context.CommandString],
                BaseCommandException => result.Exception.Message,
                _ => null
            };
            if (errorMessage != null)
            {
                await context.Client.SendTextMessageAsync(context.Update.Message.Chat.Id, errorMessage, parseMode: ParseMode.MarkdownV2);
            }
        }
    }
    
    private async Task OnStateExecuted(StateInfo? stateInfo, ModuleContext context, Result result)
    {
        if (!result.Success)
        {
            var errorMessage = result.Exception switch
            {
                BadArgs badArgs => _l["TooFewArguments"],
                TypeConvertException typeConvert => _l["TypeConvertException", typeConvert.ErrorReason, typeConvert.Position + 1],
                ValidationError validation => _l["ValidationError", validation.Message, validation.Position + 1],
                UnknownCommand => _l["UnknownState"],
                BaseCommandException => result.Exception.Message,
                _ => null
            };
            if (errorMessage != null)
            {
                await context.Client.SendTextMessageAsync(context.Update.Message.Chat.Id, errorMessage, parseMode: ParseMode.MarkdownV2);
            }
        }
    }
    
    private async Task OnCallbackExecuted(CallbackQueryHandlerInfo? callbackQueryHandlerInfo, ModuleContext context, Result result)
    {
        if (!result.Success)
        {
            var errorMessage = result.Exception switch
            {
                UnknownCommand unknownCommand => $"Unknown callback query **{context.CommandString}**",
                TypeConvertException typeConvert => _l["TypeConvertException", typeConvert.ErrorReason, typeConvert.Position + 1],
                CallbackQueryHandlerBadPath badPath => $"Parameter {badPath.ParameterInfo.Name} not present in {badPath.Path}",
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