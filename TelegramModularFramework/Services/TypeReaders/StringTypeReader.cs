using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public class StringTypeReader: ITypeReader
{
    public Type Type => typeof(string);
    
    public async Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input)
    {
        return TypeReaderResult.FromSuccess(input);
    }
}