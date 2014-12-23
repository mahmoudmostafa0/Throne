using System;
using System.Collections.Concurrent;

namespace Throne.Framework.Collections
{
    public sealed class ObjectPool<T> : ProducerConsumerCollectionBase<T>
    {
        private readonly Func<T> _generator;

        public ObjectPool(Func<T> generator)
            : this(generator, new ConcurrentQueue<T>())
        {
        }

        private ObjectPool(Func<T> generator, IProducerConsumerCollection<T> collection)
            : base(collection)
        {
            _generator = generator;
        }

        public void Drop(T item)
        {
            TryAdd(item);
        }

        public T Get()
        {
            T obj;
            return !TryTake(out obj) ? _generator() : obj;
        }
    }
}