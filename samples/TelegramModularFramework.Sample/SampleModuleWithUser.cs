using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Sample;

public class SampleModuleWithUser: TelegramModule
{
    private readonly UsersService _service;
    public string UserData { get; set; }

    public SampleModuleWithUser(UsersService service)
    {
        _service = service;
    }

    public async override Task HandlePreExecution(HandlerInfoBase info)
    {
        if (_service.Users.TryGetValue(Context.Update.Message.From.Id, out var data))
        {
            UserData = data;
        }
        else
        {
            UserData = "";
        }
    }

    [Command]
    [Summary("Sets user data")]
    public async Task Set(string input)
    {
        _service.Users[Context.Update.Message.From.Id] = input;
        await ReplyAsync("Successfully changed");
    }
    
    [Command]
    [Summary("Gets user data")]
    public async Task Get()
    {
        await ReplyAsync($"User data: {UserData}");
    }
}