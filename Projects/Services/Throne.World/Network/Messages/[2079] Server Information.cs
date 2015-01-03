using System;
using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public class ServerInfo : WorldPacket
    {
        private const Int16 SIZE = 16;


        public ServerInfo()
            : base(PacketTypes.ServerInfo, SIZE)
        {
        }
    }
}