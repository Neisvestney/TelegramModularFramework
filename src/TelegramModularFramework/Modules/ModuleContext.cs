using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramModularFramework.Services;

namespace TelegramModularFramework.Modules;


/// <summary>
/// Object which contains context for module
/// </summary>
public class ModuleContext
{
    /// <summary>
    /// Telegram client that received update
    /// </summary>
    public ITelegramBotClient Client { get; }
    
    /// <summary>
    /// ModulesService that executes current command or action
    /// </summary>
    public TelegramModulesService ModulesService { get; }
    
    /// <summary>
    /// Received update
    /// </summary>
    public Update Update { get; }
    
    /// <summary>
    /// Everything after command string
    /// Null if Action or Callback Query
    /// </summary>
    public string? CommandArgs { get; }
    
    /// <summary>
    /// Command string line '/test'
    /// </summary>
    public string CommandString { get; }
    
    /// <summary>
    /// Full name of module group.
    /// Null if handler not exists.
    /// </summary>
    public string? Group { get; }
    
    /// <summary>
    /// Circular reference to Module.
    /// Null if handler not exists.
    /// </summary>
    public TelegramModule? Module { get; }
    
    /// <summary>
    /// <see cref="TelegramModularFramework.Modules.CommandInfo"/>,
    /// <see cref="TelegramModularFramework.Modules.ActionInfo"/>,
    /// <see cref="TelegramModularFramework.Modules.StateInfo"/> or
    /// <see cref="TelegramModularFramework.Modules.CallbackQueryHandlerInfo"/>
    /// instance.
    /// Null if handler not exists.
    /// </summary>
    public HandlerInfoBase? HandlerInfo { get; }

    public ModuleContext(ITelegramBotClient client, TelegramModulesService modulesService, Update update, string? args, string commandString, string? group, TelegramModule? module, HandlerInfoBase? info)
    {
        Client = client;
        ModulesService = modulesService;
        Update = update;
        CommandArgs = args;
        CommandString = commandString;
        Group = group;
        Module = module;
        HandlerInfo = info;
    }
}