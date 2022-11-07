namespace TelegramModularFramework.Services.Exceptions;

/// <summary>
/// Arguments received by command are wrong
/// </summary>
public class BadArgs: BaseCommandException
{
    /// <summary>
    /// Index of argument from 0
    /// </summary>
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