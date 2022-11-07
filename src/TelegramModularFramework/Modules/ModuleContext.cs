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
    public ITelegramBotClient Client { get; set; }
    
    /// <summary>
    /// ModulesService that executes current command or action
    /// </summary>
    public TelegramModulesService ModulesService { get; set; }
    
    /// <summary>
    /// Received update
    /// </summary>
    public Update Update { get; set; }
    
    /// <summary>
    /// Everything after command string
    /// </summary>
    public string CommandArgs { get; set; }
    
    /// <summary>
    /// Command string line '/test'
    /// </summary>
    public string CommandString { get; set; }

    public ModuleContext(ITelegramBotClient client, TelegramModulesService modulesService, Update update, string args, string commandString)
    {
        Client = client;
        ModulesService = modulesService;
        Update = update;
        CommandArgs = args;
        CommandString = commandString;
    }
}