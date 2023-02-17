namespace TelegramModularFramework.Preconditions;

public class PreconditionResult
{
    public bool Success { get; private set; }
    public string? ErrorReason { get; private set; }
    
    public static PreconditionResult FromSuccess()
    {
        return new PreconditionResult()
        {
            Success = true
        };
    }
    
    public static PreconditionResult FromError(string errorReason)
    {
        return new PreconditionResult()
        {
            Success = false,
            ErrorReason = errorReason
        };
    }
}