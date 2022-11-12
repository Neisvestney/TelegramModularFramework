# Telegram Modular Framework
[![Nuget](https://img.shields.io/nuget/v/TelegramModularFramework)](https://www.nuget.org/packages/TelegramModularFramework/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/Neisvestney/TelegramModularFramework/Docs?label=docs)](https://neisvestney.github.io/TelegramModularFramework/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/Neisvestney/TelegramModularFramework/Publish?label=publish)](https://github.com/Neisvestney/TelegramModularFramework/actions/workflows/publish.yaml)

Framework for writing telegram bots with [Telegram.Bots](https://github.com/TelegramBots) and modules
> **In development**.

## Documentation
- [Github pages](https://neisvestney.github.io/TelegramModularFramework/)
- [Docs source](https://github.com/Neisvestney/TelegramModularFramework/tree/master/docs)
## Installation
- Add to your [C# Worker Service](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers) or Asp.Net project nuget package `TelegramModularFramework`
- Or see [Installation Guide](https://neisvestney.github.io/TelegramModularFramework/guides/getting_started/installation.html)
## Features
- Commands
  - Handling text messages starting with `/`
  - Parsing strings to method arguments
  - Automatic sets bot commands list
- Actions
  - Handling other text messages
- States
  - Handles all messages when stage activated through other handlers 
  - Passing string value to method or parsing strings to method arguments
  - Nested states
- Callback Queries
  - Handling queries by paths with dynamic path parameters
  - Parameters type converting
- [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- Dependency injection in modules
- Long pooling
- ~~WebHooks~~ _(In development)_
- ~~Localization~~ _(In development)_
## Samples
- [Commands, actions, stages and callbacks](https://github.com/Neisvestney/TelegramModularFramework/tree/master/samples/TelegramModularFramework.Sample)