using System;
using System.Collections.Generic;
using System.Linq;
using Throne.Shared.Runtime;

namespace Throne.Shared.Threading.Actors
{
    public sealed class ActorContext : IDisposableResource
    {
        private static readonly Lazy<ActorContext> _globalLazy = new Lazy<ActorContext>(() =>
        {
            int count = Environment.ProcessorCount;
            return new ActorContext(count);
        });

        private readonly List<ActorThread> _threads;

        public ActorContext(Int32 threadCount)
        {
            _threads = new List<ActorThread>(threadCount);
            for (var current = 0; current < threadCount; current++)
                _threads.Add(new ActorThread());
        }

        public static ActorContext Global
        {
            get { return _globalLazy.Value; }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            InternalDispose();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        private ActorThread PickThread()
        {
            //TODO: Try to pick actor threads based on lowest current stack clear time.
            var sched =
                _threads.Aggregate((acc, current) => current.ActorCount < acc.ActorCount ? current : acc);
            return sched;
        }

        internal ActorThread RegisterActor(Actor actor)
        {
            var thread = PickThread();
            thread.AddActor(actor);
            return thread;
        }

        ~ActorContext()
        {
            InternalDispose();
        }

        private void InternalDispose()
        {
            foreach (var thread in _threads)
                thread.Dispose();
        }
    }
}