﻿using TelegramModularFramework.Services.State;

namespace TelegramModularFramework.Services;

public class TelegramModulesConfiguration
{
    public IStateHolder StateHolder { get; set; } = new MemoryStateHolder();
}