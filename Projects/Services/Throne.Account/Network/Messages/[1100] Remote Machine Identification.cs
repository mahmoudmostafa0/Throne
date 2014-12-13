using System;
using Throne.Login.Network.Handling;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Transmission;

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