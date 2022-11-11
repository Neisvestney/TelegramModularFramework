---
title: Callback Queries
uid: Guides.Handlers.CallbackQueries
---

# Callback Queries

Handles [Telegram Callback Queries](https://core.telegram.org/bots/api#callbackquery)
from [Inline Keyboard](https://telegrambots.github.io/book/2/reply-markup.html#callback-buttons)
or from other sources  
Query data value used as path to route event to handler

## Adding callback query handler

Create in your @Guides.TelegramModule public medthod
with @TelegramModularFramework.Attributes.CallbackQueryHandlerAttribute
and pass path argument to it without starting and ending `/`  
You can add dynamic parts with `{name:regex}` format  
If `regex` empty then `*` pattern used

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: BaseTelegramModule
{
    [CallbackQueryHandler("set/{index:*}")]
    public async Task HandleNumber(int index)
    {
        await EditMessageTextAsync($"Number: {index}");
    }
}
```

If module in [Group](xref:TelegramModularFramework.Attributes.GroupAttribute)
then group path added to @TelegramModularFramework.Attributes.CallbackQueryHandlerAttribute path

```csharp
[Group("sample")]
public class SampleState: BaseTelegramModule
{
    [CallbackQueryHandler("test")]
    public async Task Handle()
    {
        await EditMessageTextAsync($"Handle");
    }
}
```
Result path is `/sample/test`

## Arguments

All dynamic parts from path can be passed to medthod by its names  
Values convertes with @Guides.TypeReadrs

## Sending buttons

Sending buttons done by sending replaty message with repltyMarkup in
other [Handler](xref:Guides.Handlers.Introduction)
```csharp
var replyMarkup = new InlineKeyboardMarkup(new []
{
    InlineKeyboardButton.WithCallbackData("Display name", "/path/to/callback"),      
});

await ReplyAsync("Select:", replyMarkup: replyMarkup);
```

## Run Mode

Run mode can be specified with @TelegramModularFramework.Attributes.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary use @TelegramModularFramework.Attributes.SummaryAttribute

```csharp
[Summary("Do things")]
```

## On Callback Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.CallbackExecuted event
to handle command post execution

### Exceptions

If callback executed unsuccesfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.TypeConvertException
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @TelegramModularFramework.Services.Exceptions.CallbackQueryHandlerBadPath
- @System.Exception