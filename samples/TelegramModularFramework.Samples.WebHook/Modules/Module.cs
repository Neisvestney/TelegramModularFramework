using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Samples.WebHook.Modules;

public class Module: BaseTelegramModule
{
    [Command]
    public async Task Start()
    {
        await ReplyAsync($"Welcome!");
    }
}