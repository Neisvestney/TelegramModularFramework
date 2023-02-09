using System.Web;

namespace TelegramModularFramework;

public static class UrlExtensions
{
    public static string ToQueryString(this IDictionary<string, object> dictionary)
    {
        return "?" + string.Join("&", dictionary.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}