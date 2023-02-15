using Microsoft.Extensions.Localization;

namespace TelegramModularFramework.Localization;

public class TypeReadersMessagesStringLocalizer: IStringLocalizer<TypeReadersMessages>
{
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }

    public LocalizedString this[string name] => new(name, TypeReadersMessages.ResourceManager.GetString(name) ?? "");

    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(TypeReadersMessages.ResourceManager.GetString(name) ?? "", arguments));
}