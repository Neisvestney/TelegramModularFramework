using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace TelegramModularFramework.Services.TypeReaders;

public class TypeReaderResult
{
    public bool Success { get; private set; }
    
    public object? Result { get; private set; }
    
    public string? ErrorReason { get; private set; }

    public static TypeReaderResult FromSuccess(object result)
    {
        return new TypeReaderResult()
        {
            Success = true,
            Result = result
        };
    }
    
    public static TypeReaderResult FromError(string errorReason)
    {
        return new TypeReaderResult()
        {
            Success = false,
            ErrorReason = errorReason
        };
    }
}