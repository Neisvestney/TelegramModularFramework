using Telegram.Bot;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample;

public class SampleModule: BaseTelegramModule
{
    public SampleModule()
    {
        
    }

    [Command("test")]
    public async Task TestCommand(string input, int? number = 1, string optional = "Test")
    {
        await ReplyAsync($"Test {input} {number + 1} {optional}");
    }
    
    [Command("test2")]
    public async Task TestCommand(int? number)
    {
        await ReplyAsync($"Test2 {number + 1}");
    }

    [Command]
    public async Task Info()
    {
        await ReplyAsync("Info");
    }
    
    [Command]
    public async Task TestException()
    {
        throw new BaseCommandException("Test Exception");
    }
    
    [Command]
    [RunMode(RunMode.Async)]
    public async Task Long()
    {
        await Task.Delay(5000);
        await ReplyAsync("Info");
    }
}