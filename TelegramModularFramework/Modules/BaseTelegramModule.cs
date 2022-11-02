using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramModularFramework.Modules;

public class  BaseTelegramModule
{
    public ModuleContext Context { get; set; }

    public async Task<Message> ReplyAsync(
        string text,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool? disableNotification = default,
        bool? protectContent = default,
        int? replyToMessageId = default,
        bool? allowSendingWithoutReply = default,
        IReplyMarkup? replyMarkup = default,
        CancellationToken cancellationToken = default)
    {
        return await Context.Client.SendTextMessageAsync(
            Context.Update.Message.Chat.Id,
            text,
            parseMode,
            entities,
            disableWebPagePreview,
            disableNotification,
            protectContent,
            replyToMessageId,
            allowSendingWithoutReply,
            replyMarkup,
            cancellationToken
        );
    }

    public Task ChangeState(string path) => Context.ModulesService.ChangeStateAsync(Context.Update.Message.Chat.Id, path);
}