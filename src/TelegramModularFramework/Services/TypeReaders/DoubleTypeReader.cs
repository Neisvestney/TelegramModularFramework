using System.Globalization;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public class DoubleTypeReader: ITypeReader
{
    public Type Type => typeof(double);
    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var output))
        {
            return TypeReaderResult.FromSuccess(output);
        }
        else
        {
            return TypeReaderResult.FromError("Not a number");
        }
    }
}