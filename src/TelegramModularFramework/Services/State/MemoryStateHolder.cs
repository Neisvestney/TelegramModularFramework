namespace TelegramModularFramework.Services.State;

public class MemoryStateHolder: IStateHolder
{
    private Dictionary<long, string> _state = new();

    public async Task SetState(long userId, string state)
    {
        _state[userId] = state;
    }

    public async Task<string?> GetState(long userId)
    {
        if (_state.TryGetValue(userId, out var state))
        {
            return state;
        }
        else
        {
            return null;
        }
    }
}