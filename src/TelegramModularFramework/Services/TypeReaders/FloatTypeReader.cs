using System.Globalization;
using Microsoft.Extensions.Localization;
using TelegramModularFramework.Localization;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public class FloatTypeReader: ITypeReader
{
    private readonly IStringLocalizer<TypeReadersMessages> _l;
    public Type Type => typeof(float);
    
    public FloatTypeReader(IStringLocalizer<TypeReadersMessages> l)
    {
        _l = l;
    }
    
    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var output))
        {
            return TypeReaderResult.FromSuccess(output);
        }
        else
        {
            return TypeReaderResult.FromError(_l["NotAFloat"]);
        }
    }
}