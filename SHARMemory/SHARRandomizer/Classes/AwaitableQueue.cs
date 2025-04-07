using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHARRandomizer.Classes
{
    /* Implementation of a thread safe queue that will allow us to wait until an item is received and then dequeue */
    public class AwaitableQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            _signal.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _signal.WaitAsync(cancellationToken);
            if (_queue.TryDequeue(out T item))
            {
                return item;
            }
            // This should not happen because the semaphore was released.
            throw new InvalidOperationException("Queue signaled an available item, but none was found.");
        }
    }
}
