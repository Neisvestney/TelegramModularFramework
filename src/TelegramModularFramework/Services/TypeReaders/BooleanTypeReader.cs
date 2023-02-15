using Microsoft.Extensions.Localization;
using TelegramModularFramework.Localization;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public class BooleanTypeReader : ITypeReader
{
    private readonly IStringLocalizer<TypeReadersMessages> _l;
    
    public Type Type => typeof(bool);
    
    public BooleanTypeReader(IStringLocalizer<TypeReadersMessages> l)
    {
        _l = l;
    }

    private List<string> _trueValues = new()
    {
        "true",
        "1",
        "yes",
        "y",
    };

    private List<string> _falseValues = new()
    {
        "false",
        "0",
        "no",
        "n",
    };

    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (_trueValues.Contains(input))
        {
            return TypeReaderResult.FromSuccess(true);
        }
        else if (_falseValues.Contains(input))
        {
            return TypeReaderResult.FromSuccess(false);
        }
        else
        {
            return TypeReaderResult.FromError(_l["NotABoolean"]);
        }
    }
}