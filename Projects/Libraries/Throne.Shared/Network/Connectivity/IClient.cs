using System;
using System.Net;
using Throne.Shared.Logging;
using Throne.Shared.Network.Security;
using Throne.Shared.Threading.Actors;

namespace Throne.Shared.Network.Connectivity
{
    public interface IClient : IActor
    {
        INetworkCipher StreamCipher { get; }

        INetworkCipher TransferCipher { get; set; }

        IPAddress ClientAddress { get; }

        Int32 SocketId { get; }

        dynamic UserData { get; set; }

        LogProxy Log { get; }
        void Disconnect();
        void DisconnectWithMessage(Byte[] message);
        void Send(byte[] value);
    }
}