using System.Globalization;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.Globalization;

public interface ICultureInfoUpdater
{
    public CultureInfo GetCultureInfo(ModuleContext context);
}