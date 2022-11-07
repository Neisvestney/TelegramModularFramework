namespace TelegramModularFramework;

public static class AsynchronousEventExtensions
{
    public static Task InvokeAsync<TEventArgs1, TEventArgs2, TEventArgs3>(this Func<TEventArgs1, TEventArgs2, TEventArgs3, Task> handlers, TEventArgs1 args1, TEventArgs2 args2, TEventArgs3 args3)
    {
        if (handlers != null)
        {
            return Task.WhenAll(handlers.GetInvocationList()
                .OfType<Func<TEventArgs1, TEventArgs2, TEventArgs3, Task>>()
                .Select(h => h(args1, args2, args3)));
        }

        return Task.CompletedTask;
    }
}