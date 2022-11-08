using System.Reflection;
using TelegramModularFramework.Services.Utils;

namespace TelegramModularFramework.Services.Exceptions;

public class CallbackQueryHandlerBadPath: BaseCommandException
{
    public IEnumerable<PathPart> PathParts { get; set; }
    public ParameterInfo ParameterInfo { get; set; }
    public string Path { get; set; }

    public CallbackQueryHandlerBadPath(IEnumerable<PathPart> pathParts, ParameterInfo parameterInfo, string path)
    {
        PathParts = pathParts;
        ParameterInfo = parameterInfo;
        Path = path;
    }
}