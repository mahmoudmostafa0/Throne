using System;
using Throne.Shared.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public class ServerInfo : WorldPacket
    {
        private const Int16 SIZE = 16;


        public ServerInfo()
            : base(PacketTypes.ServerInfo, SIZE)
        {
            //Unknown size 4 at offset 4 is unnecessary.
        }
    }
}