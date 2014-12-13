using System;
using System.Reflection;
using System.Threading;

using Throne.Shared.Exceptions;
using Throne.Shared.Logging;
using Throne.Shared.Threading.Actors;

namespace Throne.Shared.Threading
{
    public abstract class ActorApplication<T> : SingletonActor<T>
        where T : ActorApplication<T>
    {
        public const int UpdateDelay = 50;

        protected readonly ActorTimer _updateTimer;
        public LogProxy Log;

        private DateTime _lastUpdate;

        private bool _shouldStop;

        protected ActorApplication()
        {
            Log = new LogProxy(GetType().Name);
            
            _updateTimer = new ActorTimer(this, UpdateCallback, TimeSpan.FromMilliseconds(UpdateDelay), UpdateDelay);
            _lastUpdate = DateTime.Now;
        }

        public event EventHandler Shutdown;

        protected override void Dispose(bool disposing)
        {
            _updateTimer.Dispose();

            base.Dispose(disposing);
        }

        public virtual void Start(string[] args)
        {
            try { OnStart(args); }
            catch (Exception ex) { ExceptionManager.RegisterException(ex); }
   

            GC.Collect(2, GCCollectionMode.Optimized);
        }

        public void Stop()
        {
            try
            {
                var shutdownEvent = Shutdown;
                if (shutdownEvent != null)
                    shutdownEvent(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
            }

            try
            {
                OnStop();
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
            }

            GC.Collect();

            _shouldStop = true;
        }

        protected virtual void OnStart(string[] args)
        {
        }

        protected virtual void OnStop()
        {
        }

        private void UpdateCallback()
        {
            if (_shouldStop)
            {
                _updateTimer.Change(Timeout.InfiniteTimeSpan);
                return;
            }

            DateTime now = DateTime.Now;
            TimeSpan diff = now - _lastUpdate;
            _lastUpdate = now;

            Pulse(diff);
        }

        protected virtual void Pulse(TimeSpan diff)
        {
        }

        private static string AssemblyVersion
        {
            get
            {
                var ver = Assembly.GetEntryAssembly().GetName().Version;
                return "{0}.{1}.{2}.{3}".Interpolate(ver.Major, ver.Minor, ver.Build, ver.Revision);
            }
        }

        public override string ToString()
        {
            return "Throne {0} (v{1})".Interpolate(GetType().Name, AssemblyVersion);
        }
    }
}