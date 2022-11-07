using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public class IntTypeReader: ITypeReader
{
    public Type Type => typeof(int);
    
    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        if (int.TryParse(input, out var output))
        {
            return TypeReaderResult.FromSuccess(output);
        }
        else
        {
            return TypeReaderResult.FromError("Not a number");
        }
    }
}