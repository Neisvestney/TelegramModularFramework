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
}