using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

public interface ITypeReader
{
    public Type Type { get; }
    public Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input);
}