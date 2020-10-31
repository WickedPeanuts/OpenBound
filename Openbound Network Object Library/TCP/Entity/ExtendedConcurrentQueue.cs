using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Network_Object_Library.TCP.Entity
{
    /// <summary>
    /// ConcurrentQueue wrapper. It calls a <see cref="OnEnqueueAction"/> whenever a new item is enqueued.
    /// </summary>
    public class ExtendedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public Action<T> OnEnqueueAction;

        public ExtendedConcurrentQueue(){}

        public ExtendedConcurrentQueue(Action<T> onEnqueueAction)
        {
            OnEnqueueAction = onEnqueueAction;
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnEnqueueAction?.Invoke(item);
        }
    }
}
