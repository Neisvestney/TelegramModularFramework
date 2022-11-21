namespace TelegramModularFramework.Services.State;


/// <summary>
/// Stores states in memory
/// </summary>
public class MemoryStateHolder: IStateHolder
{
    private Dictionary<long, string> _state = new();

    /// <inheritdoc/>
    public async Task SetState(long userId, string state)
    {
        _state[userId] = state;
    }

    /// <inheritdoc/>
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