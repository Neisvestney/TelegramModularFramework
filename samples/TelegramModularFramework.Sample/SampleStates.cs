using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample; 

public class SampleStates: BaseTelegramModule
{
    [Command]
    [Action]
    [Summary("Sets name")]
    public async Task EnterName()
    {
        await ReplyAsync($"Enter name:");
        await ChangeState("sample");
    }
    
    [Group("sample")]
    public class SampleState: BaseTelegramModule
    {
        [StateHandler]
        public async Task HandleState(string input)
        {
            if (input == null) throw new ValidationError("No text in message", nameof(input), 0);
            await ReplyAsync($"You entered: {input}");
            await ReplyAsync($"Enter age:");
            ChangeState("age");
        }
        
        [Group("age")]
        public class SampleAgeState: BaseTelegramModule
        {
            [StateHandler(parseArgs:true)]
            public async Task HandleState(int input)
            {
                if (input < 1) throw new ValidationError("Age cannot be lower then 1", nameof(input), 0);
                await ReplyAsync($"You entered: {input}");
                ChangeState("/");
            }
        }
    }
}