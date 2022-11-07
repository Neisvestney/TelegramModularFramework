namespace TelegramModularFramework.Services.Exceptions;


/// <summary>
/// Some argument has wrong value
/// </summary>
public class ValidationError: BaseCommandException
{
    public string Field { get; set; }
    public int Position { get; set; }

    public ValidationError(string message, string field, int posistion): base(message)
    {
        Field = field;
        Position = posistion;
    }
}