using TelegramModularFramework.Preconditions;

namespace TelegramModularFramework.Services.Exceptions;

public class UnmetPrecondition: BaseCommandException
{
    public PreconditionAttribute Precondition { get; }

    public UnmetPrecondition(PreconditionAttribute precondition, string message): base(message)
    {
        Precondition = precondition;
    }
}