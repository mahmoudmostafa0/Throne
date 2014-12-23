/* 
 * ::: Thanks to Spirited Fang for the original copy of this class.
 */

using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Runtime;

namespace Throne.Framework.Network.Transmission
{
    public abstract class Packet : Processor, IDisposableResource
    {
        protected Packet(byte[] receivedArray)
            : base(receivedArray)
        {
            ReadType();
        }

        protected Packet(int arrayLength, int writtenLenth)
            : base(arrayLength)
        {
            Array = new byte[arrayLength];
            WriteHeader(writtenLenth);
        }

        protected Packet(IConvertible type, int len)
            : base(len)
        {
            TypeId = (short) type;
            WriteHeader(len);
        }

        protected Packet(int len)
            : base(len)
        {
        }

        protected Packet()
        {
        }

        /// <summary>
        ///     Returns the packet type ID.
        /// </summary>
        public Int16 TypeId { get; protected set; }


        /// <summary>
        ///     True if the packet has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return Array == null; }
        }

        #region Processing

        public virtual Boolean Read(IClient client)
        {
            return false;
        }

        public virtual void Handle(IClient client)
        {
        }

        public override sealed unsafe void WriteHeader(int length)
        {
            base.WriteHeader(length);

            fixed (byte* ptr = Array)
                *(short*) (ptr + 2) = TypeId;

            Seek(4);
        }

        public override sealed unsafe void ReadType()
        {
            fixed (byte* ptr = Array)
                TypeId = *(short*) (ptr + 2);

            Seek(4);
        }

        /// <summary> This method converts the operator class into an unsigned byte array. </summary>
        /// <param name="structure">The structure being converted into an unsigned byte array.</param>
        /// <returns>This method returns the array built by the packet class and it's inheritance.</returns>
        public static implicit operator byte[](Packet structure)
        {
            return structure.Build();
        }

        #endregion

        #region Logging

        private void LogError(string message, IClient client, bool disconnect)
        {
            Log.Warn("Protocol violation by client {0}: {1} in {2}".Interpolate(client, message, GetType().Name));

            if (disconnect)
                client.Disconnect();
        }

        protected bool InvalidValue<T>(IClient client, String field, T value, bool disconnect = true)
        {
            LogError("{0} was invalid (was {1})".Interpolate(field, value), client, disconnect);
            return disconnect;
        }

        protected bool InvalidValue<T>(IClient client, String field, T value, T expected, bool disconnect = true)
        {
            LogError("{0} was expected to be {1} (was {2})".Interpolate(field, expected, value), client, disconnect);
            return disconnect;
        }

        protected bool InvalidValueRange<T>(IClient client, String field, T value, T expectedUpper,
            T expectedLower,
            bool disconnect = true)
        {
            LogError(
                "{0} was expected to be between {1} and {2} (was {3})".Interpolate(field, expectedLower, expectedUpper,
                    value), client, disconnect);
            return disconnect;
        }

        #endregion
    }
}