using System;
using System.Diagnostics.Contracts;

namespace Throne.Framework.Exceptions
{
    /// <summary>
    ///     Provides data for the <see cref="ExceptionManager.ExceptionOccurred" /> event.
    /// </summary>
    public sealed class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(ExceptionInfo info)
        {
            Contract.Requires(info != null);

            Info = info;
        }

        /// <summary>
        ///     The information object of the exception that occurred.
        /// </summary>
        public ExceptionInfo Info { get; private set; }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Info != null);
        }
    }
}