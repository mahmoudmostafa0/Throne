using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Throne.Framework.Exceptions;
using Throne.Framework.Security;

namespace Throne.Framework.Threading.Actors
{
    public class Actor : RestrictedObject, IActor
    {
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

            _msgIterator = EnumerateMessages();
            _mainIterator = Main();

            Thread = Context.RegisterActor(this);
            Thread.Disposed += OnDisposed;
        }

        internal bool IsActive { get; set; }

        internal ActorThread Thread { get; set; }

        public ActorContext Context { get; private set; }
        public bool IsDisposed { get; private set; }

        public void PostAsync(Action msg)
        {
            _msgQueue.Enqueue(msg);

            Action tmp;
            while (!_msgQueue.TryPeek(out tmp))
                if (_msgQueue.Count == 0)
                    return; // The message was processed immediately, and we can just return.

            if (msg != tmp) return;
            Thread.AddActor(this);
        }

        /// <summary>
        ///     Post a non-blocking awaited action.
        /// </summary>
        /// <param name="msg"></param>
        public Task PostWait(Action msg)
        {
            var eventHandle = new SemaphoreSlim(0);

            try
            {
                //limit execution time to 5 seconds.
                return eventHandle.WaitAsync(5000);
            }
            finally
            {
                PostAsync(() =>
                {
                    msg();
                    eventHandle.Release();
                });
            }
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
            Dispose(false); //disposed unmanaged
        }

        private void InternalDispose()
        {
            if (IsDisposed)
                return;

            Dispose(true);
            IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        internal bool ProcessMessages()
        {
            _msgIterator.MoveNext();
            // No disposing here.
            return _msgQueue.Count > 0;
        }

        internal bool ProcessMain()
        {
            bool result = _mainIterator.MoveNext();

            // Happens if a yield break occurs.
            if (!result)
                return false;

            Operation operation = _mainIterator.Current;

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
                    Operation? op = OnMessage(msg);
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
        /// <summary>
        ///     Post a non-blocking action.
        /// </summary>
        /// <param name="msg"></param>
        public void PostAsync(Action<TThis> msg)
        {
            PostAsync(() => msg((TThis) this));
        }

        /// <summary>
        ///     Post a non-blocking awaited action.
        /// </summary>
        /// <param name="msg"></param>
        public Task PostWait(Action<TThis> msg)
        {
            return PostWait(() => msg((TThis) this));
        }
    }
}