using Microsoft.Extensions.Localization;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Localization;

namespace TelegramModularFramework.Services.TypeReaders;

public class IntTypeReader: ITypeReader
{
    private readonly IStringLocalizer<TypeReadersMessages> _l;
    public Type Type => typeof(int);

    public IntTypeReader(IStringLocalizer<TypeReadersMessages> l)
    {
        _l = l;
    }

    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (int.TryParse(input, out var output))
        {
            return TypeReaderResult.FromSuccess(output);
        }
        else
        {
            return TypeReaderResult.FromError(_l["NotANumber"]);
        }
    }
}