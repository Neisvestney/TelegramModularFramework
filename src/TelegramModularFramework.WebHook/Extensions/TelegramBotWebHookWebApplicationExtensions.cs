using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramModularFramework.Services;
using TelegramModularFramework.WebHook.Services;

namespace TelegramModularFramework.WebHook;

public static class TelegramBotWebHookWebApplicationExtensions
{
    /// <summary>
    /// Creates route for receiving WebHooks with ASP.Net minimal API
    /// </summary>
    /// <param name="app">App to configure</param>
    public static void MapTelegramWebHook(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<TelegramBotWebHookHostConfiguration>>().Value;
        
        app.MapPost(options.Route,
            async (HttpRequest request,
                [FromServices] ILogger<TelegramBotWebHookHostedService> logger,
                [FromServices] TelegramBotEvents events,
                [FromServices] ITelegramBotClient botClient,
                CancellationToken cancellationToken) =>
            {
                if (!SecretTokenValid(request, options.SecretToken))
                {
                    return Results.BadRequest();
                }
                
                if (!request.HasJsonContentType())
                {
                    return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
                }

                using StreamReader sr = new StreamReader(request.Body);

                var content = await sr.ReadToEndAsync();
                var update = JsonConvert.DeserializeObject<Update>(content);

                if (update != null) await events.HandleUpdateAsync(botClient, update, cancellationToken);
                else return Results.BadRequest();

                return Results.Ok();
            });
    }

    private static bool SecretTokenValid(HttpRequest request, string token)
    {
        var isSecretTokenProvided = request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var secretTokenHeader);
        if (!isSecretTokenProvided) return false;

        return string.Equals(secretTokenHeader, token, StringComparison.Ordinal);
    }
}