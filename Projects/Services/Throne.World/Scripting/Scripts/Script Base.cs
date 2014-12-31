using System;
using System.Reflection;
using Throne.Framework;
using Throne.Framework.Exceptions;
using Throne.Framework.Logging;
using Throne.Framework.Runtime;

namespace Throne.World.Scripting.Scripts
{
    public abstract class ScriptBase : IDisposableResource, IAutoLoader, IScript
    {
        protected LogProxy Log;

        protected ScriptBase()
        {
            Log = new LogProxy("{0}->{1}".Interpolate(GetType().BaseType.Name, GetType().Name));
        }

        /// <summary>
        ///     Unsubscribes from all auto subscribed events.
        /// </summary>
        public virtual void Dispose()
        {
            MethodInfo[] methods = GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                object[] attrs = method.GetCustomAttributes(typeof(OnAttribute), false);
                if (attrs.Length == 0)
                    continue;

                var attr = attrs[0] as OnAttribute;

                EventInfo eventHandlerInfo = WorldServer.Instance.Events.GetType().GetEvent(attr.Event);
                if (eventHandlerInfo == null)
                    continue;

                try
                {
                    eventHandlerInfo.RemoveEventHandler(WorldServer.Instance.Events,
                        Delegate.CreateDelegate(eventHandlerInfo.EventHandlerType, this, method));
                }
                catch (Exception ex)
                {
                    ExceptionManager.RegisterException(ex);
                }
            }

            CleanUp();
        }

        public bool IsDisposed { get; private set; }

        public virtual bool Init()
        {
            Load();
            return true;
        }

        public virtual void Load()
        {
        }

        /// <summary>
        ///     Adds subscriptions based on "On" attribute on methods.
        /// </summary>
        public void AutoLoad()
        {
            Type type = GetType();
            MethodInfo[] methods = GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                object[] attrs = method.GetCustomAttributes(typeof(OnAttribute), false);
                if (attrs.Length == 0)
                    continue;

                var attr = attrs[0] as OnAttribute;

                EventInfo eventHandlerInfo = WorldServer.Instance.Events.GetType().GetEvent(attr.Event);
                if (eventHandlerInfo == null)
                {
                    LogManager.Debug("AutoLoadEvents: Unknown event '{0}' on '{1}.{2}'.".Interpolate(attr.Event, type.Name, method.Name));
                    continue;
                }

                try
                {
                    eventHandlerInfo.AddEventHandler(WorldServer.Instance.Events,
                        Delegate.CreateDelegate(eventHandlerInfo.EventHandlerType, this, method));
                }
                catch (Exception ex)
                {
                    ExceptionManager.RegisterException(ex);
                }
            }
        }

        /// <summary>
        ///     Called from Dispose, use for cleaning up before reload.
        /// </summary>
        protected virtual void CleanUp()
        {
        }

        public static implicit operator Boolean(ScriptBase scr)
        {
            return scr != null;
        }
    }
}