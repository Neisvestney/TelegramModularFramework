using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramModularFramework.Services;

namespace TelegramModularFramework.Modules;

public class ModuleContext
{
    public ITelegramBotClient Client { get; set; }
    public TelegramModulesService ModulesService { get; set; }
    public Update Update { get; set; }
    public string CommandArgs { get; set; }
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