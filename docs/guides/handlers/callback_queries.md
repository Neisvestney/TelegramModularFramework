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

Create in your @Guides.TelegramModule public method
with @TelegramModularFramework.Modules.CallbackQueryHandlerAttribute
and pass path argument to it without starting and ending `/`  
You can add dynamic parts with `{name:regex}` format  
If `regex` empty then `*` pattern used

```csharp
using TelegramModularFramework.Modules;

public class SampleModule: TelegramModule
{
    [CallbackQueryHandler("set/{index:*}")]
    public async Task HandleNumber(int index)
    {
        await EditMessageTextAsync($"Number: {index}");
    }
}
```

If module in [Group](xref:TelegramModularFramework.Modules.GroupAttribute)
then group path added to @TelegramModularFramework.Modules.CallbackQueryHandlerAttribute path

```csharp
[Group("sample")]
public class SampleState: TelegramModule
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

All dynamic parts from path can be passed to method by its names  
Path query params also parsed and can be passed   
Values converts with @Guides.TypeReaders

## Sending buttons

Sending buttons done by sending replay message with replyMarkup in
other [Handler](xref:Guides.Handlers.Introduction)
```csharp
var replyMarkup = new InlineKeyboardMarkup(new []
{
    InlineKeyboardButton.WithCallbackData("Display name", "/path/to/callback"),      
});

await ReplyAsync("Select:", replyMarkup: replyMarkup);
```

## Run Mode

Run mode can be specified with @TelegramModularFramework.Modules.RunModeAttribute  
@TelegramModularFramework.Modules.RunMode.Sync - Default. Commands executes in order  
@TelegramModularFramework.Modules.RunMode.Async - Commands executes asynchronously

## Summary

To add summary use @TelegramModularFramework.Modules.SummaryAttribute

```csharp
[Summary("Do things")]
```

## On Callback Executed

Subscribe to @TelegramModularFramework.Services.TelegramModulesService.CallbackExecuted event
to handle command post execution

### Exceptions

If callback executed unsuccessfully handle @TelegramModularFramework.Modules.Result.Exception  
Possible exception:

- @TelegramModularFramework.Services.Exceptions.UnknownCommand
- @TelegramModularFramework.Services.Exceptions.TypeConvertException
- @TelegramModularFramework.Services.Exceptions.BaseCommandException
- @TelegramModularFramework.Services.Exceptions.CallbackQueryHandlerBadPath
- @System.Exception