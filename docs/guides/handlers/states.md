---
title: States
uid: Guides.Handlers.States
---

# States

If previous handler calls @TelegramModularFramework.Modules.BaseTelegramModule.ChangeState
then associated [State Handler](xref:TelegramModularFramework.Attributes.StateHandlerAttribute) called

## Adding command

Create in your @Guides.TelegramModule public medthod
with @TelegramModularFramework.Attributes.StateHandlerAttribute  
@Guides.TelegramModule must be in [Group](xref:TelegramModularFramework.Attributes.GroupAttribute)

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: BaseTelegramModule
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
            await ChangeState("/"); // Return back
        }
    }
}
```

State path generates from module group

## Arguments

If @TelegramModularFramework.Attributes.StateHandlerAttribute.ParseArgs is false 
[State Handler](xref:TelegramModularFramework.Attributes.StateHandlerAttribute)
retrieve on string argument equals to message text  

If @TelegramModularFramework.Attributes.StateHandlerAttribute.ParseArgs is true
arguments parsed as [commands](xref:Guides.Handlers.Commands#arguments) do

```csharp
[StateHandler(parseArgs:true)]
```

## Run Mode

Run mode can be specified with @TelegramModularFramework.Attributes.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary to command use @TelegramModularFramework.Attributes.SummaryAttribute

```csharp
[Summary("Do things")]
```

## On State Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.StateExecuted event
to handle state post execution

### Exceptions

If state executed unsuccesfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.BadArgs
- @TelegramModularFramework.Services.Exceptions.TypeConvertException
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @System.Exception