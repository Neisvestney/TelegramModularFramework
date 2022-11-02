namespace TelegramModularFramework.Services.Utils;

public class StringSplitter: IStringSplitter
{
    public List<string> Split(string? args)
    {
        var result = args != null
            ? args
                .Split('"')
                .Select((element, index) => index % 2 == 0 // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // Split the item
                    : new string[] { element }) // Keep the entire item
                .SelectMany(element => element)
            : new List<string>();

        return result.ToList();
    }
}

public interface IStringSplitter
{
    public List<string> Split(string? args);
}