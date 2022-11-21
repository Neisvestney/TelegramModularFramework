namespace TelegramModularFramework.Services.State;


/// <summary>
/// The service that used for holding long <see langword="and" /> <see langword="string"/> keypairs for memorize states of chats
/// </summary>
public interface IStateHolder
{
    /// <summary>
    /// Sets state value for user
    /// </summary>
    /// <param name="userId">Number represents user</param>
    /// <param name="state">String value to store</param>
    /// <returns></returns>
    public Task SetState(long userId, string state);
    
    /// <summary>
    /// Retrieves previously set state
    /// </summary>
    /// <param name="userId">Number represents user</param>
    /// <returns>Nullable string state</returns>
    public Task<string?> GetState(long userId);
}