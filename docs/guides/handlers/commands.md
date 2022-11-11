---
title: Commands
uid: Guides.Handlers.Commands
---

# Commands

Text messages starting with `/`

## Adding command

Create in your @Guides.TelegramModule public medthod
with @TelegramModularFramework.Attributes.CommandAttribute

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: BaseTelegramModule
{
    [Command]
    public async Task Start()
    {
        await ReplyAsync($"Welcome!");
    }
}
```

By default command name generates from method name `CommandName -> /commandname`  
You can specify name by passing `name` argument to @TelegramModularFramework.Attributes.CommandAttribute  
Name cannot contain spaces

```csharp
[Command("test")]
```

## Arguments

String after command name parsed as command arguments  
Adding parameters to a command is done by adding parameters to the command handler method

```csharp
[Command]
public async Task Command(int number, float numberFloat, double numberDouble, bool boolean)
{
    await ReplyAsync($"{number} {numberFloat} {numberDouble} {boolean}");
}
```

If string contains spaces it can be wrapped in quotes
`/command "String with spaces"`  
Avalible typereaders and guide how to make custom one are present in @Guides.TypeReadrs

## Run Mode

Run mode can be specified with @TelegramModularFramework.Attributes.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary to command use @TelegramModularFramework.Attributes.SummaryAttribute

```csharp
[Summary("Do things")]
```

## SetMyCommands

You can call @TelegramModularFramework.Services.TelegramModulesService.SetMyCommands to set
list of visible to bot  
Command must have [summary](xref:Guides.Handlers.Commands#summary)
To hide set `hideFromList` argument to @TelegramModularFramework.Attributes.CommandAttribute

```csharp
[Command(hideFromList: true)]
```

## On Command Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.CommandExecuted event
to handle command post execution

### Exceptions

If command executed unsuccesfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.BadArgs
- @TelegramModularFramework.Services.Exceptions.TypeConvertException
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @System.Exception