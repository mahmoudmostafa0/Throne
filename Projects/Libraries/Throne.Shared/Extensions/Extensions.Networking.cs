using System.Net.Sockets;
using Throne.Shared.Network.Communication;

namespace Throne.Shared
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
