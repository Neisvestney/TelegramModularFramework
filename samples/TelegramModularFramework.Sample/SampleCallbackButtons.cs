using Telegram.Bot.Types.ReplyMarkups;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample;

public class SampleCallbackButtons : BaseTelegramModule
{
    [Command]
    [Action]
    [Summary("Selects page")]
    public async Task SelectPage()
    {
        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Prev", UrlFor<SampleState>("HandlePrev")),
            InlineKeyboardButton.WithCallbackData("1", UrlFor<SampleState>("HandleNumber", new { index = 1 })),
            InlineKeyboardButton.WithCallbackData("2", UrlFor<SampleState>("HandleNumber", new { index = 2 })),
            InlineKeyboardButton.WithCallbackData("Next", UrlFor<SampleState>("HandleNext")),
            InlineKeyboardButton.WithCallbackData("Test", UrlFor<SampleState>("HandleTest", new { index = 1, other = "test", query = "test2" })),
        });

        await ReplyAsync("Select page:", replyMarkup: replyMarkup);
    }

    [Group("page")]
    public class SampleState : BaseTelegramModule
    {
        [CallbackQueryHandler("prev")]
        public async Task HandlePrev()
        {
            await EditMessageTextAsync($"Prev");
        }

        [CallbackQueryHandler("next")]
        public async Task HandleNext()
        {
            await EditMessageTextAsync($"Next");
        }

        [CallbackQueryHandler("set/{index}")]
        public async Task HandleNumber(int index)
        {
            await EditMessageTextAsync($"Page: {index}");
        }

        [CallbackQueryHandler("test/{index:*}/{other:*}")]
        public async Task HandleTest(string index, string other, string query)
        {
            await EditMessageTextAsync($"Test: {index} {other} {query}");
        }
    }
}