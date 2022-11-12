---
title: Telegram Module
uid: Guides.TelegramModule
---

# Telegram Module

Telegram Module is a class inherit from @TelegramModularFramework.Modules.BaseTelegramModule  
Used to define [Handlers](xref:Guides.Handlers.Introduction)

## Context

Module has @TelegramModularFramework.Modules.BaseTelegramModule.Context field with
current client, update object, command and args strings

Other fields in [Api Rerefence](xref:TelegramModularFramework.Modules.BaseTelegramModule)

## Methods

To reply to chat from which came current event you can use
@TelegramModularFramework.Modules.BaseTelegramModule.ReplyAsync  
To change current chat state there are @TelegramModularFramework.Modules.BaseTelegramModule.ChangeState medhod

Other methods in [Api Rerefence](xref:TelegramModularFramework.Modules.BaseTelegramModule)

## Groups

> [!WARNING]
> **In development.** Commands and actions don't respect groups

To group your [Handlers](xref:Guides.Handlers.Introduction) you can nest modeles with
@TelegramModularFramework.Attributes.GroupAttribute

```csharp
public class SampleStates: BaseTelegramModule
{
    [Group("sample")]
    public class SampleGroup: BaseTelegramModule
    {
        [Group("test")]
        public class TestGroup: BaseTelegramModule
        {
        
        }
    }
}
```

@TelegramModularFramework.Modules.ModuleContext.Group field for `TestGroup` is `/sample/test`

## Dependency Injection

All Modules has DI support  
For more see @Guides.DependencyInjection guide
