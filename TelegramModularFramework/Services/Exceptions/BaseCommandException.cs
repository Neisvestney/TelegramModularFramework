namespace TelegramModularFramework.Services.Exceptions;

public class BaseCommandException: Exception
{
    public BaseCommandException() {}

    public BaseCommandException(string message): base(message) { }
}