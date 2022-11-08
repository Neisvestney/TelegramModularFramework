namespace TelegramModularFramework.Attributes;

/// <summary>
/// Registers method that used for handling <see cref="Telegram.Bot.Types.Enums.UpdateType.CallbackQuery"/> updates
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CallbackQueryHandlerAttribute: Attribute
{
    /// <summary>
    /// Path pattern with regex support.
    /// Currently only one path argument supported.
    /// </summary>
    public string Path { get; set; }

    public CallbackQueryHandlerAttribute(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be empty");
        Path = path;
    }
}