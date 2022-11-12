---
title: Introduction
uid: Guides.Handlers.Introduction
---

# Introduction

Framework handles different types of messages and events  
All handlers must be defined in [Modules](xref:Guides.TelegramModule)  
Available handlers:

- @Guides.Handlers.Commands
- @Guides.Handlers.Actions
- @Guides.Handlers.States
- @Guides.Handlers.CallbackQueries

One method can be handler for different events:
```csharp
[Action("Action")]
[Command("command")]
public async Task Hybrid()
{
    ReplyAsync("Hybrid");
}
```