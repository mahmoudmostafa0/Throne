using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Throne.Framework.Exceptions
{
    /// <summary>
    ///     Holds information describing an exception.
    /// </summary>
    public sealed class ExceptionInfo
    {
        internal ExceptionInfo(Exception exception)
        {
            Contract.Requires(exception != null);

            Exception = exception;
            OccurrenceTime = DateTime.Now;
        }

        /// <summary>
        ///     The exception that occurred.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///     The time of the exception.
        /// </summary>
        public DateTime OccurrenceTime { [UsedImplicitly] get; private set; }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Exception != null);
        }
    }
}