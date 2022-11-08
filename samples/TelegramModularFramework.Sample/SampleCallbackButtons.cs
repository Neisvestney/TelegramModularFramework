using Telegram.Bot.Types.ReplyMarkups;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;

namespace TelegramModularFramework.Sample; 

public class SampleCallbackButtons: BaseTelegramModule
{
    [Command]
    [Action]
    [Summary("Selects page")]
    public async Task SelectPage()
    {
        var replyMarkup = new InlineKeyboardMarkup(new []
        {
            InlineKeyboardButton.WithCallbackData("Prev", "/page/prev"),    
            InlineKeyboardButton.WithCallbackData("1", "/page/set/1"),    
            InlineKeyboardButton.WithCallbackData("2", "/page/set/2"),    
            InlineKeyboardButton.WithCallbackData("Next", "/page/next"),    
            InlineKeyboardButton.WithCallbackData("Test", "/page/test/1/2"),    
        });

        await ReplyAsync("Select page:", replyMarkup: replyMarkup);
    }
    
    [Group("page")]
    public class SampleState: BaseTelegramModule
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
        public async Task HandleTest(string index, string other)
        {
            await EditMessageTextAsync($"Test: {index} {other}");
        }
    }
}