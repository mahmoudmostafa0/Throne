using System.Net.Sockets;
using Throne.Framework.Network.Communication;

namespace Throne.Framework
{
    public static partial class Extensions
    {
        public static void Release(this SocketAsyncEventArgs saea)
        {
            SocketAsyncEventArgsPool.Release(saea);
        }

        public static void Terminate(this Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket.Dispose();
        }
    }
}
