using Microsoft.Extensions.Localization;
using TelegramModularFramework.Localization;
using TelegramModularFramework.Services.Globalization;
using TelegramModularFramework.Services.State;

namespace TelegramModularFramework.Services;

public class TelegramModulesConfiguration
{
    /// <summary>
    /// StateHolder to use
    /// </summary>
    public IStateHolder StateHolder { get; set; } = new MemoryStateHolder();
    public ICultureInfoUpdater CultureInfoUpdater { get; set; } = new UserLanguageCultureInfoUpdater();
    public IStringLocalizer<TypeReadersMessages> TypeReadersMessagesStringLocalizer { get; set; } = new TypeReadersMessagesStringLocalizer();
}