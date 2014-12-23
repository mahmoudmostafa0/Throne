using System;
using System.Threading;
using Throne.Framework.Runtime;

namespace Throne.Framework.Threading.Actors
{
    public class ActorTimer : IDisposableResource
    {
        /// <summary>
        /// Using Timer from the Threading namespace because
        /// it provides ability to wait for the first execution.
        /// We wont be using Timers.Timer for this reason.
        /// </summary>
        private readonly Timer _timer;

        public AutoResetEvent Auto;

        public ActorTimer(IActor target, Action callback, TimeSpan delay, int period = Timeout.Infinite)
        {
            TargetActor = target;
            Callback = callback;
            Auto = new AutoResetEvent(false);
            _timer = new Timer(TimerCallback, Auto, delay, TimeSpan.FromMilliseconds(period));
        }
        public ActorTimer(IActor target, Action callback, TimeSpan delay, TimeSpan period)
        {
            TargetActor = target;
            Callback = callback;
            Auto = new AutoResetEvent(false);
            _timer = new Timer(TimerCallback, Auto, delay, period);
        }

        public IActor TargetActor { get; private set; }

        public Action Callback { get; private set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            InternalDispose();
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~ActorTimer()
        {
            InternalDispose();
        }

        private void InternalDispose()
        {
            _timer.Dispose();
        }

        public void Change(TimeSpan delay, int period = Timeout.Infinite)
        {
            _timer.Change(delay, TimeSpan.FromMilliseconds(period));
        }

        public void Change(TimeSpan delay, TimeSpan period)
        {
            _timer.Change(delay, period);
        }

        protected virtual void TimerCallback(object state)
        {
            TargetActor.PostAsync(Callback);
        }
    }
}