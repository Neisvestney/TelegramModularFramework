---
title: Localization
uid: Guides.Localization
---

# Localization

Framework has localization support.  
Before handler
executes [Current Culture Info](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.currentculture?view=net-6.0)
updates with @TelegramModularFramework.Services.Globalization.ICultureInfoUpdater

## ICultureInfoUpdater

By default, used @TelegramModularFramework.Services.Globalization.UserLanguageCultureInfoUpdater which
get culture from telegram user lang code.

To use own write implementation of @TelegramModularFramework.Services.Globalization.ICultureInfoUpdater and
configure [TelegramModulesService](xref:TelegramModularFramework.TelegramBotHostBuilderExtensions.AddTelegramModulesService(Microsoft.Extensions.Hosting.IHostBuilder,Action{Microsoft.Extensions.Hosting.HostBuilderContext,TelegramModularFramework.Services.TelegramModulesConfiguration}))
with custom @TelegramModularFramework.Services.Globalization.ICultureInfoUpdater

```csharp
.AddTelegramModulesService((context, c) =>
{
    c.CultureInfoUpdater = new CustomCultureInfoUpdater();
})
```

## Default Error Messages
TelegramModularFramework.Localization.DefaultErrorMessages is resource file contains
some common messages.
You can use it to localize errors in post execution event.  
See [Samples](xref:Guides.Introduction#samples) for more info.

## Type Readers

Default Type Readers already have localization

## Contribution

If you want to add new localization you can add new [Resource](https://github.com/Neisvestney/TelegramModularFramework/blob/master/src/TelegramModularFramework/Resources/) with pull request or
add localization to [CSV FIle](https://docs.google.com/spreadsheets/d/1pXsZrf8hRi32zTOl8LbpmmFyb0zCS17EeMeVNBWAIG4/edit?usp=sharing) and open issue with updated file