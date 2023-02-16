using Telegram.Bot;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Samples.WebHook.Modules;

public class Module: TelegramModule
{
    [Command]
    public async Task Start()
    {
        // In preview version of Telegram.Bot SendTextMessageAsync signature has been changed. 
        await ReplyAsync("Welcome");
    }
}