---
title: States
uid: Guides.Handlers.States
---

# States

If previous handler calls @TelegramModularFramework.Modules.TelegramModule.ChangeState
then associated [State Handler](xref:TelegramModularFramework.Modules.StateHandlerAttribute) called

## Adding command

Create in your @Guides.TelegramModule public medthod
with @TelegramModularFramework.Modules.StateHandlerAttribute  
@Guides.TelegramModule must be in [Group](xref:TelegramModularFramework.Modules.GroupAttribute)

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: TelegramModule
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
    public class SampleState: TelegramModule
    {
        [StateHandler]
        public async Task HandleState(string input)
        {
            if (input == null) throw new ValidationError("No text in message", nameof(input), 0);
            await ReplyAsync($"You entered: {input}");
            await ChangeState("/"); // Return back
        }
    }
}
```

State path generates from module group

## Arguments

If @TelegramModularFramework.Modules.StateHandlerAttribute.ParseArgs is false 
[State Handler](xref:TelegramModularFramework.Modules.StateHandlerAttribute)
retrieve on string argument equals to message text  

If @TelegramModularFramework.Modules.StateHandlerAttribute.ParseArgs is true
arguments parsed as [commands](xref:Guides.Handlers.Commands#arguments) do

```csharp
[StateHandler(parseArgs:true)]
```

## Run Mode

Run mode can be specified with @TelegramModularFramework.Modules.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary to command use @TelegramModularFramework.Modules.SummaryAttribute

```csharp
[Summary("Do things")]
```

## On State Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.StateExecuted event
to handle state post execution

### Exceptions

If state executed unsuccessfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.BadArgs
- @TelegramModularFramework.Services.Exceptions.TypeConvertException
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @System.Exception