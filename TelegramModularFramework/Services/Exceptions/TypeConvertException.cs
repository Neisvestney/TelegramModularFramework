using System.Reflection;

namespace TelegramModularFramework.Services.Exceptions;

public class TypeConvertException: BaseCommandException
{
    public string ErrorReason { get; }
    public ParameterInfo ParameterInfo { get; }
    public int Position { get; }

    public TypeConvertException(string errorReason, ParameterInfo parameterInfo, int position)
    {
        ErrorReason = errorReason;
        ParameterInfo = parameterInfo;
        Position = position;
    }
}