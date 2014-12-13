using System;
using System.Net.Sockets;
using Throne.Shared.Collections;

namespace Throne.Shared.Network.Communication
{
    public static class SocketAsyncEventArgsPool
    {
        private static readonly ObjectPool<SocketAsyncEventArgs> ObjectPool =
            new ObjectPool<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs());

        public static SocketAsyncEventArgs Acquire(EventHandler<SocketAsyncEventArgs> e = null)
        {
            var obj = ObjectPool.Get();
            obj.Completed += e;
            return obj;
        }

        public static void Release(SocketAsyncEventArgs arg)
        {
            if (arg != null)
                ObjectPool.Drop(arg);
        }
    }
}