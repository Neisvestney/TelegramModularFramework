---
title: Dependency Injection
uid: Guides.DependencyInjection
---

# Dependency Injection

All [Modules](xref:Guides.TelegramModule) have DI support and activates through [ActivatorUtilities](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.activatorutilities?view=dotnet-plat-ext-6.0)
with scoped service provider

```csharp
public class SampleModule: BaseTelegramModule
{
    private readonly TelegramModulesService _modulesService;
    private readonly SampleService _service;

    public SampleModule(TelegramModulesService modulesService, SampleService service)
    {
        _modulesService = modulesService;
        _service = service;
    }
 }
```
