namespace TelegramModularFramework.Services.Exceptions;

public class BadArgs: BaseCommandException
{
    public int Position { get; }
    public int Required { get; }
    public int Provided { get; }

    public BadArgs(int position, int required, int provided)
    {
        Position = position;
        Required = required;
        Provided = provided;
    }
}