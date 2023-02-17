using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Modules;

namespace TelegramModularFramework.Preconditions;

public class RequireChatType: PreconditionAttribute
{
    public ChatType ChatType { get; }
    
    public RequireChatType(ChatType chatType)
    {
        ChatType = chatType;
    }
    
    public async override Task<PreconditionResult> CheckPreconditionAsync(ModuleContext context, IServiceProvider serviceProvider)
    {
        var chatType = context.Update.Message?.Chat?.Type ?? context.Update.InlineQuery?.ChatType;
        if (chatType != null && ChatType.HasFlag(chatType))
            return PreconditionResult.FromSuccess();
        else
            return PreconditionResult.FromError("RequireChatType");
    }
}