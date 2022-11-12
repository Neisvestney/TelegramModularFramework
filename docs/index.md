---
uid: Root.Index
title: Home
---

# Telegram Modular Framework
[![Nuget](https://img.shields.io/nuget/v/TelegramModularFramework)](https://www.nuget.org/packages/TelegramModularFramework/)
[![Source](https://img.shields.io/badge/source-github-green)](https://github.com/Neisvestney/TelegramModularFramework/)  
Framework for writing telegram bots with [Telegram.Bots](https://github.com/TelegramBots) and modules
> [!WARNING]
> **In development.**
# Quick start
- @Guides.GettingStarted.ProjectCreation
# Features
- [Commands](xref:Guides.Handlers.Commands)
    - Handling text messages starting with `/`
    - Parsing strings to method arguments
    - Authomatic sets bot commands list
- [Actions](xref:Guides.Handlers.Actions)
    - Handling other text messages
- [States](xref:Guides.Handlers.States)
    - Handles all messages when stage activated through other handlers
    - Passing string value to method or parsing strings to method arguments
    - Nested states
- [Callback Queries](xref:Guides.Handlers.CallbackQueries)
    - Handling queries by paths with dynamic path parameters
    - Parameters type convertion
- [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Dependency injection in modules](xref:Guides.DependencyInjection)
- Long pooling
- ~~WebHooks~~ _(In develompment)_
- ~~Localization~~ _(In develompment)_
# Samples
- [Commands, actions, stages and callbacks](https://github.com/Neisvestney/TelegramModularFramework/tree/master/samples/TelegramModularFramework.Sample)