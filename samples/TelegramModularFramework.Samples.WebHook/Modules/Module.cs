using Telegram.Bot;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Samples.WebHook.Modules;

public class Module: BaseTelegramModule
{
    [Command]
    public async Task Start()
    {
        // In preview version of Telegram.Bot SendTextMessageAsync signature has been changed. 
        await Context.Client.SendTextMessageAsync(Context.Update.Message.Chat.Id, "Welcome!");
    }
}