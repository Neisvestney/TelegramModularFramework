using System.ComponentModel.DataAnnotations;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Preconditions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class PreconditionAttribute : Attribute
{
    public abstract Task<PreconditionResult> CheckPreconditionAsync(ModuleContext context, IServiceProvider serviceProvider);
}