using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace SHARMemory.SHAR.Events;

/// <summary>
/// An asynchronous event handler.
/// </summary>
/// <typeparam name="TEventArgs">The event's arguments.</typeparam>
/// <param name="sender">The <see cref="Memory"/> that triggered the event.</param>
/// <param name="e">The event's arguments.</param>
/// <param name="token">The cancellation token.</param>
/// <returns>A <see cref="Task"/>.</returns>
public delegate Task AsyncEventHandler<TEventArgs>(Memory sender, TEventArgs e, CancellationToken token);

internal static class AsyncEventHandlerExtensions
{
    public static bool HasSubscribers<TEventArgs>(this AsyncEventHandler<TEventArgs> handler) => handler?.GetInvocationList()?.Length > 0;

    public static async Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, Memory sender, TEventArgs args, CancellationToken token)
    {
        await Task.Run(async () =>
        {
            var delegates = handler?.GetInvocationList();
            if (delegates?.Length > 0)
            {
                var tasks = delegates.Cast<AsyncEventHandler<TEventArgs>>().Select(e => Task.Run(async () => await e.Invoke(sender, args, token)));
                await Task.WhenAll(tasks);
            }
        }).ConfigureAwait(false);
    }
}