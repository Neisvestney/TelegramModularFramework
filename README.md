# Telegram Modular Framework
Framework for writting telegram bots with [Telegram.Bots](https://github.com/TelegramBots) and modules
> **In development**. No nuget package. Will be available soon.
## Documentation
- Github pages soon
- [Docs source](https://github.com/Neisvestney/TelegramModularFramework/tree/master/docs)
## Installation
- There will be nuget
## Features
- Commands
  - Handling text messages starting with `/`
  - Parsing strings to method arguments
  - Authomatic sets bot commands list
- Actions
  - Handling other text messages
- States
  - Handles all messages when stage activated through other handlers 
  - Passing string value to method or parsing strings to method arguments
  - Nested states
- Callback Queries
  - Handling queries by paths with dynamic path parameters
  - Parameters type convertion
- [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- Dependency injection in modules
- Long pooling
- ~~WebHooks~~ _(In develompment)_
- ~~Localization~~ _(In develompment)_
## Samples
- [Commands, actions, stages and callbacks](https://github.com/Neisvestney/TelegramModularFramework/tree/master/samples/TelegramModularFramework.Sample)