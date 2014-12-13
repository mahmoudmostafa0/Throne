using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Throne.Shared.Exceptions;
using Throne.Shared.Security;

namespace Throne.Shared.Threading.Actors
{
    public class Actor : RestrictedObject, IActor
    {
        private readonly AutoResetEvent _disposeEvent;
        private readonly IEnumerator<Operation> _mainIterator;
        private readonly IEnumerator<Operation> _msgIterator;

        private readonly ConcurrentQueue<Action> _msgQueue = new ConcurrentQueue<Action>();

        public Actor()
            : this(ActorContext.Global)
        {
        }

        public Actor(ActorContext context)
        {
            Context = context;
            _disposeEvent = new AutoResetEvent(false);

            _msgIterator = EnumerateMessages();
            _mainIterator = Main();

            Thread = Context.RegisterActor(this);
            Thread.Disposed += OnDisposed;
        }

        internal bool IsActive { get; set; }

        internal ActorThread Thread { get; set; }

        public ActorContext Context { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Join()
        {
            _disposeEvent.WaitOne();
        }

        public void PostAsync(Action msg)
        {
            _msgQueue.Enqueue(msg);

            Action tmp;
            while (!_msgQueue.TryPeek(out tmp))
                if (_msgQueue.Count == 0)
                    return; // The message was processed immediately, and we can just return.

            if (msg == tmp)
            // The message was sent while the actor was idle; restart it to continue processing.
                Thread.AddActor(this);
        }

        public IWaitable PostWait(Action msg)
        {
            var eventHandle = new AutoResetEvent(false);

            PostAsync(() =>
            {
                msg();

                // Signal that message processing has happened.
                eventHandle.Set();
            });

            return new ActorWaitHandle(eventHandle);
        }

        public void Dispose()
        {
            PostAsync(InternalDispose);
        }


        private void OnDisposed(object sender, EventArgs args)
        {
            Thread.Disposed -= OnDisposed;

            // No guarantee is made about where an actor is disposed!
            InternalDispose();
        }

        ~Actor()
        {
            Console.WriteLine("Actor disposed.");
            Dispose(false);//disposed unmanaged
        }

        private void InternalDispose()
        {
            if (IsDisposed)
                return;

            Dispose(true);
            IsDisposed = true;
            _disposeEvent.Set();
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        internal bool ProcessMessages()
        {
            _msgIterator.MoveNext();

            return _msgQueue.Count > 0;
        }

        internal bool ProcessMain()
        {
            var result = _mainIterator.MoveNext();

            // Happens if a yield break occurs.
            if (!result)
                return false;

            var operation = _mainIterator.Current;

            if (operation == Operation.Dispose)
                Dispose();

            return operation == Operation.Continue;
        }

        protected virtual IEnumerator<Operation> Main()
        {
            yield break; // No main by default.
        }

        private IEnumerator<Operation> EnumerateMessages()
        {
            while (true)
            {
                Action msg;
                if (_msgQueue.TryDequeue(out msg))
                {
                    var op = OnMessage(msg);
                    if (op != null)
                        yield return (Operation) op;
                }
                yield return Operation.Continue;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        protected virtual Operation? OnMessage(Action msg)
        {
            try
            {
                msg();
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
                return Operation.Dispose;
            }
            return null;
        }
    }

    public abstract class Actor<TThis> : Actor, IActor<TThis>
        where TThis : Actor<TThis>
    {
        public void PostAsync(Action<TThis> msg)
        {
            PostAsync(() => msg((TThis) this));
        }

        public IWaitable PostWait(Action<TThis> msg)
        {
            return PostWait(() => msg((TThis) this));
        }
    }
}