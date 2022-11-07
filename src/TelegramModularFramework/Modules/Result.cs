namespace TelegramModularFramework.Modules;

public class Result
{
    public bool Success { get; private set; }
    public Exception? Exception { get; private set; }

    public static Result FromSuccess()
    {
        return new Result()
        {
            Success = true
        };
    }
    
    public static Result FromError(Exception exception)
    {
        return new Result()
        {
            Success = false,
            Exception = exception
        };
    }
}
