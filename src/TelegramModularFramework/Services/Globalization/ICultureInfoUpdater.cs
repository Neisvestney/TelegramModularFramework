using System.Globalization;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.Globalization;

/// <summary>
/// Service that used for retrieving user <see cref="System.Globalization.CultureInfo"/>
/// </summary>
public interface ICultureInfoUpdater
{
    /// <summary>
    /// Retrieve <see cref="System.Globalization.CultureInfo"/> from current <see cref="TelegramModularFramework.Modules.ModuleContext"/>
    /// </summary>
    /// <param name="context"><see cref="TelegramModularFramework.Modules.ModuleContext"/></param>
    /// <returns><see cref="System.Globalization.CultureInfo"/></returns>
    public CultureInfo GetCultureInfo(ModuleContext context);
}