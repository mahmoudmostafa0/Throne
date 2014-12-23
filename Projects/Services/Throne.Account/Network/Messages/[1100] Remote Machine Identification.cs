using System;
using Throne.Login.Network.Handling;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;

namespace Throne.Login.Network.Messages
{
    [AuthenticationPacketHandler(PacketTypes.RemoteMachineIdentification)]
    public sealed class RemoteMachineIdentification : AuthenticationPacket
    {
        public RemoteMachineIdentification(Byte[] array) : base(array)
        {
        }

        public override void Handle(IClient client)
        {
            client.Disconnect();
        }
    }
}