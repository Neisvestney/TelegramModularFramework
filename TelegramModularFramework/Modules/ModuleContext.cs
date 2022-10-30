using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramModularFramework.Modules;

public class ModuleContext
{
    public ITelegramBotClient Client { get; set; }
    public Update Update { get; set; }
    public string[] CommandArgs { get; set; }
    public string CommandString { get; set; }

    public ModuleContext(ITelegramBotClient client, Update update, string[] args, string commandString)
    {
        Client = client;
        Update = update;
        CommandArgs = args;
        CommandString = commandString;
    }
}