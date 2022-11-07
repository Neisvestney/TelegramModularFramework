namespace TelegramModularFramework.Services.State;


/// <summary>
/// The service that used for holding long <see langword="and" /> <see langword="string"/> keypairs for memorize states of chats
/// </summary>
public interface IStateHolder
{
    public Task SetState(long userId, string state);
    public Task<string?> GetState(long userId);
}