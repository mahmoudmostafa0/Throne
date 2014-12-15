using System;
using Throne.Shared;
using Throne.World.Security;

namespace Throne.World
{
    public class EventManager
    {
        /// <summary>
        /// Raised when there's a security violation
        /// </summary>
        public event Action<SecurityViolationEventArgs> SecurityViolation;
        public void OnSecurityViolation(SecurityViolationEventArgs args) { SecurityViolation.Raise(args); }

    }

    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Raises event with thread and null-ref safety.
        /// </summary>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }
    }
}
