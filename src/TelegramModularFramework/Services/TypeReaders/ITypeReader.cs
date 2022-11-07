using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Services.TypeReaders;

/// <summary>
/// Interface for TypeRears used by <see cref="TelegramModularFramework.Services.TelegramModulesService"/> to parse arguments from string
/// </summary>
public interface ITypeReader
{
    /// <summary>
    /// The type that the TypeReader reads
    /// </summary>
    public Type Type { get; }
    
    /// <summary>
    /// Must convert string to <see cref="Type"/> or return unsuccessful <see cref="TelegramModularFramework.Services.TypeReaders.TypeReaderResult"/>
    /// </summary>
    /// <param name="context">Module context</param>
    /// <param name="input">String input. Can be with spaces</param>
    /// <returns> <see cref="TelegramModularFramework.Services.TypeReaders.TypeReaderResult.FromSuccess"/> or  <see cref="TelegramModularFramework.Services.TypeReaders.TypeReaderResult.FromError"/></returns>
    public Task<TypeReaderResult> ReadTypeAsync(ModuleContext context, string input);
}