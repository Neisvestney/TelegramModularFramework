namespace TelegramModularFramework.Attributes;

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