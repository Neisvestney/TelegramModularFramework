using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample; 

public class SampleStates: TelegramModule
{
    [Command]
    [Action]
    [Summary("Sets name")]
    public async Task EnterName()
    {
        await ReplyAsync($"Enter name:");
        await ChangeStateAsync(UrlFor<SampleState>());
    }
    
    [Group("sample")]
    public class SampleState: TelegramModule
    {
        [StateHandler]
        public async Task HandleState(string input)
        {
            if (input == null) throw new ValidationError("No text in message", nameof(input), 0);
            await ReplyAsync($"You entered: {input}");
            await ReplyAsync($"Enter age:");
            await ChangeStateAsync(UrlFor<SampleAgeState>());
        }
        
        [Group("age")]
        public class SampleAgeState: TelegramModule
        {
            [StateHandler(parseArgs:true)]
            public async Task HandleState(int input)
            {
                if (input < 1) throw new ValidationError("Age cannot be lower then 1", nameof(input), 0);
                await ReplyAsync($"You entered: {input}");
                await ExitFromStateAsync();
            }
        }
    }
}