---
title: Telegram Module
uid: Guides.TelegramModule
---

# Telegram Module

Telegram Module is a class inherit from @TelegramModularFramework.Modules.TelegramModule  
Used to define [Handlers](xref:Guides.Handlers.Introduction)

## Context

Module has @TelegramModularFramework.Modules.TelegramModule.Context field with
current client, update object, command and args strings

Other fields in [Api Rerefence](xref:TelegramModularFramework.Modules.TelegramModule)

## Methods

To reply to chat from which came current event you can use
@TelegramModularFramework.Modules.TelegramModule.ReplyAsync  
To change current chat state there are @TelegramModularFramework.Modules.TelegramModule.ChangeState medhod

Other methods in [Api Rerefence](xref:TelegramModularFramework.Modules.TelegramModule)

## Groups

> [!WARNING]
> **In development.** Commands and actions don't respect groups

To group your [Handlers](xref:Guides.Handlers.Introduction) you can nest modeles with
@TelegramModularFramework.Modules.GroupAttribute

```csharp
public class SampleStates: TelegramModule
{
    [Group("sample")]
    public class SampleGroup: TelegramModule
    {
        [Group("test")]
        public class TestGroup: TelegramModule
        {
        
        }
    }
}
```

@TelegramModularFramework.Modules.ModuleContext.Group field for `TestGroup` is `/sample/test`

## Dependency Injection

All Modules has DI support  
For more see @Guides.DependencyInjection guide
