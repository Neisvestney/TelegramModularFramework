namespace TelegramModularFramework.Services.State;

public interface IStateHolder
{
    public Task SetState(long userId, string state);
    public Task<string?> GetState(long userId);
}