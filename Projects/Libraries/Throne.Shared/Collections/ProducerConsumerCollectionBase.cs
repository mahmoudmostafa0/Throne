using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Throne.Shared.Collections
{
    [Serializable]
    public abstract class ProducerConsumerCollectionBase<T> : IProducerConsumerCollection<T>
    {
        private readonly IProducerConsumerCollection<T> _contained;

        protected ProducerConsumerCollectionBase(IProducerConsumerCollection<T> contained)
        {
            if (contained == null)
                throw new ArgumentNullException("contained");
            _contained = contained;
        }

        protected IProducerConsumerCollection<T> ContainedCollection
        {
            get { return _contained; }
        }

        public int Count
        {
            get { return _contained.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return _contained.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _contained.SyncRoot; }
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            return TryAdd(item);
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryTake(out item);
        }

        public T[] ToArray()
        {
            return _contained.ToArray();
        }

        public void CopyTo(T[] array, int index)
        {
            _contained.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            _contained.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _contained.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual bool TryAdd(T item)
        {
            return _contained.TryAdd(item);
        }

        protected virtual bool TryTake(out T item)
        {
            return _contained.TryTake(out item);
        }
    }
}