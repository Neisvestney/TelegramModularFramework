namespace TelegramModularFramework.Services.Utils;


/// <summary>
/// Callback query data splited to parts
/// </summary>
public class PathPart
{
    /// <summary>
    /// Name of part.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Regex string to math this path part.
    /// Same as name if not <see cref="Dynamic"/>
    /// </summary>
    public string Template { get; set; }
    
    /// <summary>
    /// Dynamic path part has different <see cref="Name"/> and <see cref="Template"/> and can be passed to
    /// handler as parameter with same <see cref="Name"/>.
    /// </summary>
    public bool Dynamic { get; set; }
}