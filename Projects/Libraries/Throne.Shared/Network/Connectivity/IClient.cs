using System;
using System.Net;
using Throne.Framework.Commands;
using Throne.Framework.Logging;
using Throne.Framework.Network.Security;
using Throne.Framework.Threading.Actors;

namespace Throne.Framework.Network.Connectivity
{
    public interface IClient : IActor, ICommandUser
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