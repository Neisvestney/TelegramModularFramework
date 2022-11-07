namespace TelegramModularFramework.Services.Exceptions;

/// <summary>
/// Base class for all exception that occurs due to bot user 
/// </summary>
public class BaseCommandException: Exception
{
    public BaseCommandException() {}

    public BaseCommandException(string message): base(message) { }
}