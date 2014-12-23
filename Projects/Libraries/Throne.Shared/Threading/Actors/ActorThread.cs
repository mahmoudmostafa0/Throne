using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Throne.Framework.Threading.Actors
{
    public sealed class ActorThread
    {
        private static volatile int _threadCount;
        private readonly List<Actor> _actors = new List<Actor>();

        private readonly AutoResetEvent _event = new AutoResetEvent(false);
        private readonly ConcurrentQueue<Actor> _newActors = new ConcurrentQueue<Actor>();

        private readonly ManualResetEventSlim _processedEvent = new ManualResetEventSlim(true);
        private readonly Thread _thread;

        private volatile bool _running = true;

        public ActorThread()
        {
            _thread = new Thread(ThreadBody)
            {
                Name = "Actor Thread {0}".Interpolate(++_threadCount),
                IsBackground = true
            };
            _thread.Start();
        }

        public event EventHandler Disposed;

        public int ActorCount
        {
            get { return _actors.Count; }
        }

        public void AddActor(Actor actor)
        {
            _newActors.Enqueue(actor);
            _event.Set();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            InternalDispose();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed { get; private set; }
        
        private void TakeNewActors()
        {
            while (_newActors.Count > 0)
            {
                Actor newActor;
                if (!_newActors.TryDequeue(out newActor))
                    continue;

                if (newActor.IsActive)
                    continue;

                newActor.IsActive = true;
                newActor.Thread = this;
                _actors.Add(newActor);
            }
        }

        private void ThreadBody()
        {
            while (_running)
            {
                _event.WaitOne();
                TakeNewActors();

                _processedEvent.Reset();

                while (_actors.Count > 0)
                {
                    TakeNewActors();

                    _actors.RemoveAll(x =>
                    {
                        if (x.IsDisposed || (!x.ProcessMain() & !x.ProcessMessages()))
                        {
                            x.IsActive = false;
                            return true;
                        }

                        return false;
                    });

                    Thread.Yield();
                }
                _processedEvent.Set();
            }
        }

        ~ActorThread()
        {
            InternalDispose();
        }

        private void InternalDispose()
        {
            _running = false;

            // Wait for processing to stop.
            _processedEvent.Wait();

            // Notify all actors that we're shutting down.
            EventHandler evnt = Disposed;
            if (evnt != null)
                evnt(this, EventArgs.Empty);
        }
    }
}