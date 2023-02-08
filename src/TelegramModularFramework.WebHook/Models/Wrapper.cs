using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TelegramModularFramework.WebHook.Models;

public class Wrapper<TModel>
{
    public Wrapper(TModel? value)
    {
        Value = value;
    }

    public TModel? Value { get; }

    public static async ValueTask<Wrapper<TModel>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        if (!context.Request.HasJsonContentType())
        {
            throw new BadHttpRequestException(
                "Request content type was not a recognized JSON content type.",
                StatusCodes.Status415UnsupportedMediaType);
        }

        using var sr = new StreamReader(context.Request.Body);
        var str = await sr.ReadToEndAsync();

        return new Wrapper<TModel>(JsonConvert.DeserializeObject<TModel>(str));
    }
}