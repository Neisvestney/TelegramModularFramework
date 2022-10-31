using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample;

public class SampleModule: BaseTelegramModule
{
    private readonly TelegramModulesService _modulesService;

    public SampleModule(TelegramModulesService modulesService)
    {
        _modulesService = modulesService;
    }

    [Command]
    public async Task Start()
    {
        var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[]
        {
            "Action",
            "Show Something"
        });
        keyboard.ResizeKeyboard = true;
        await ReplyAsync($"Welcome!", replyMarkup: keyboard);
    }

    [Command("test")]
    [Summary("Test command")]
    public async Task TestCommand(string input, int? number = 1, string optional = "Test")
    {
        await ReplyAsync($"Test {input} {number + 1} {optional}");
    }
    
    [Command("test2")]
    [Summary("Number input")]
    public async Task TestCommand2(int? number)
    {
        await ReplyAsync($"Test2 {number + 1}");
    }

    [Command]
    [Summary("Prints info")]
    public async Task Info()
    {
        await ReplyAsync("Info");
    }
    
    [Command(hideFromList: true)]
    [Summary("Throws exception")]
    public async Task TestException()
    {
        throw new BaseCommandException("Test Exception");
    }
    
    [Command]
    [Summary("Long running task")]
    [RunMode(RunMode.Async)]
    public async Task Long()
    {
        await Task.Delay(5000);
        await ReplyAsync("Info");
    }

    [Command]
    [Summary("Available commands")]
    public async Task Help()
    {
        await ReplyAsync($@"Available commands:
{string.Join("\n", _modulesService.VisibleCommands.Select(c => $"/{c.Name} - {c.Summary}"))}");
    }

    [Action]
    public async Task ShowSomething()
    {
        ReplyAsync("Something");
    }
    
    [Action("Action")]
    [Command("command")]
    public async Task Hybrid()
    {
        ReplyAsync("Hybrid");
    }
}