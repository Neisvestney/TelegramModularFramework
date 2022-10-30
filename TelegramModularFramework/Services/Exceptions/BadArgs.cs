namespace TelegramModularFramework.Services.Exceptions;

public class BadArgs: BaseCommandException
{
    public int Position { get; }

    public BadArgs(int position)
    {
        Position = position;
    }
}