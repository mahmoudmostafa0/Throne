using System;
using System.Runtime.CompilerServices;

namespace Throne.Framework.Threading
{
    public interface IWaitable : INotifyCompletion
    {
        /// <summary>
        /// Caution must be used when waiting indefinitely.
        /// Best to use <code>Wait(TimeSpan timeout)</code>
        /// </summary>
        /// <returns></returns>
        bool Wait();

        /// <summary>
        /// Wait until time is up.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        bool Wait(TimeSpan timeout);
    }
}