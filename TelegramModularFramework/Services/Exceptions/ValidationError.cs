namespace TelegramModularFramework.Services.Exceptions;

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