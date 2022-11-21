using System.Globalization;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.Globalization;

public class UserLanguageCultureInfoUpdater : ICultureInfoUpdater
{
    public CultureInfo GetCultureInfo(ModuleContext context)
    {
        var code = context.Update.Type switch
        {
            UpdateType.Message => context.Update.Message?.From?.LanguageCode,
            UpdateType.CallbackQuery => context.Update.Message?.From?.LanguageCode,
            _ => null
        };
        return CultureInfo.GetCultureInfoByIetfLanguageTag(code ?? "en");
    }
}