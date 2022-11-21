using TelegramModularFramework.Modules;
using TelegramModularFramework.Resources;

namespace TelegramModularFramework.Services.TypeReaders;

public class BooleanTypeReader : ITypeReader
{
    public Type Type => typeof(bool);

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
            return TypeReaderResult.FromError(TypeReadersMessages.NotABoolean);
        }
    }
}