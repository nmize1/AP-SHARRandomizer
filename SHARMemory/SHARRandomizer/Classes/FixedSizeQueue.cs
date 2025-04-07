using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHARRandomizer.Classes
{
    /* Implementation of a Queue with a fixed size and a print function to mimic a chat log style print */
    public class FixedSizeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        public int Cap { get; }
        public event Action<T> OnEnqueue;

        public FixedSizeQueue(int cap)
        {
            if(cap < 0)
                throw new ArgumentOutOfRangeException(nameof(cap));
            Cap = cap;
        }
        public void Enqueue(T item)
        {
            if (queue.Count >= Cap)
                queue.Dequeue();

            queue.Enqueue(item);

            OnEnqueue?.Invoke(item);
        }

        public T Dequeue()
        {
            if (queue.Count <= 0)
                throw new InvalidOperationException("Queue is empty.");

            return queue.Dequeue();
        }

        public T Peek()
        {
            if (queue.Count <= 0)
                throw new InvalidOperationException("Queue is empty.");

            return queue.Peek();
        }

        public string Print()
        {
            string ret = string.Join("\n", queue);
            while ((ret.Length > 499))
            {
                queue.Dequeue();
                ret = string.Join("\n", queue);
            }
            return ret;
        }
    }
}
