using System.Globalization;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Resources;

namespace TelegramModularFramework.Services.TypeReaders;

public class FloatTypeReader: ITypeReader
{
    public Type Type => typeof(float);
    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var output))
        {
            return TypeReaderResult.FromSuccess(output);
        }
        else
        {
            return TypeReaderResult.FromError(TypeReadersMessages.NotAFloat);
        }
    }
}