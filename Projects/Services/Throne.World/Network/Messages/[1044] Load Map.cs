using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.LoadMap)]
    public class LoadMap : WorldPacket
    {
        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public LoadMap(Byte[] array) : base(array)
        {
        }

        public LoadMap(UInt32 mapId)
            : base(PacketTypes.LoadMap, 24)
        {
            WriteBoolean(false); //teleport
            WriteBoolean(false); //unknown
            WriteBoolean(false); //ready
            WriteBoolean(false); //unknown
            WriteBoolean(true); //load
            SeekForward(3);
            WriteUInt(mapId);
        }

        public override bool Read(IClient client)
        {
            ((WorldClient) client).Character.InitializationSignal.Release();
            return true;
        }
    }
}