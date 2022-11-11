---
title: Actions
uid: Guides.Handlers.Actions
---

# Actions

Common text messages  
Can be combinated with [Keyboards](https://telegrambots.github.io/book/2/reply-markup.html#single-row-keyboard-markup)

## Adding action

Create in your @Guides.TelegramModule public medthod
with @TelegramModularFramework.Attributes.ActionAttribute

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: BaseTelegramModule
{
    [Action]
    public async Task ShowSomething()
    {
        await ReplyAsync($"Welcome!");
    }
}
```

By default action name generates from method name `ActionName -> Action Name`  
You can specify name by passing `name` argument to @TelegramModularFramework.Attributes.ActionAttribute  
Name cannot contain spaces

```csharp
[Action("Test")]
```

## Arguments

Cannot retrive arguments but as other handlers can activate @Guides.Handlers.State
to get some information

## Run Mode

Run mode can be specified with @TelegramModularFramework.Attributes.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary to command use @TelegramModularFramework.Attributes.SummaryAttribute

```csharp
[Summary("Do things")]
```

## On Action Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.ActionExecuted event
to handle action post execution

### Exceptions

If action executed unsuccesfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @System.Exception