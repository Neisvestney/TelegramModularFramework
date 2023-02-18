using System.Globalization;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.Globalization;

/// <summary>
/// Retrieves <see cref="System.Globalization.CultureInfo"/> from user <see cref="Telegram.Bot.Types.User.LanguageCode"/>
/// </summary>
public class UserLanguageCultureInfoUpdater : ICultureInfoUpdater
{
    /// <inheritdoc/>
    public CultureInfo GetCultureInfo(ModuleContext context)
    {
        var code = context.Update.Type switch
        {
            UpdateType.Message => context.Update.Message?.From?.LanguageCode,
            UpdateType.CallbackQuery => context.Update.CallbackQuery?.From.LanguageCode,
            _ => null
        };
        return CultureInfo.GetCultureInfoByIetfLanguageTag(code ?? "en");
    }
}