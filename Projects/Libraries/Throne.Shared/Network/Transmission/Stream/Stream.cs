using System;
using System.Collections.Generic;
using NHibernate.Mapping;
using Throne.Shared.Native;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Runtime;

namespace Throne.Shared.Network.Transmission.Stream
{
    /// <summary>
    ///     The cause of this class is to join multiple packets together.
    ///     As such, the client socket is able to send a single array up to 65,535 bytes long with many
    ///     individual packets. All of this while making certain we're not splitting a packet.
    ///     This improves client performance and prevent client hangs due to high packet receipt volume.
    /// </summary> 
    public unsafe class Stream : IDisposableResource
    {
        private const Int32 MAX_STREAM_SIZE = UInt16.MaxValue;

        private readonly Byte* _stream;
        private Int32 _streamPosition;

        /// <summary>
        ///     The cause of this class is to join multiple packets together.
        ///     As such, the client socket is able to send a single array up to 65,535 bytes long with many
        ///     individual packets. All of this while making certain we're not splitting a packet.
        ///     This improves client performance and prevent client hangs due to high packet receipt volume.
        /// </summary>
        public Stream()
        {
            _stream = (Byte*) NativeMethods.malloc(MAX_STREAM_SIZE);
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            NativeMethods.free(_stream);
            IsDisposed = true;
        }

        public Stream Join(Byte[] packet)
        {
            NativeMethods.memcpy(_stream + _streamPosition, packet, packet.Length);
            _streamPosition += packet.Length;
            return this;
        }

        public Stream Join(IEnumerable<Byte[]> packets)
        {
            foreach (var packet in packets)
                Join(packet);
            return this;
        }

        public static implicit operator byte[](Stream stream)
        {
            var streamBytes = new byte[stream._streamPosition];
            NativeMethods.memcpy(streamBytes, stream._stream, streamBytes.Length);
            return streamBytes;
        }

        public static Stream operator +(Stream stream, Byte[] packet)
        {
            return stream.Join(packet);
        }

        public static Stream operator +(Stream stream, IEnumerable<Byte[]> packets)
        {
            return stream.Join(packets);
        }

        public static Stream operator >(Stream stream, IClient client)
        {
            client.Send(stream);
            return stream;
        }

        public static Stream operator <(Stream stream, IClient client)
        {
            client.Send(stream);
            return stream;
        }
    }
}