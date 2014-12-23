using System;
using Throne.Framework;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Security;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Objects.Actors;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.NpcAction)]
    public sealed class NpcAction : WorldPacket
    {
        public NpcAction(Byte[] array)
            : base(array)
        {
            SeekForward(sizeof(int)); //incoming timestamp
        }

        public override bool Read(IClient client)
        {
            Character chr = ((WorldClient)client).Character;
            uint id = ReadUInt();
            short unknown1 = ReadShort();
            byte option = ReadByte();
            ushort interactionType = ReadUShort();
            string inputReturn = ReadString();


            Npc npc;
            if (!(npc = chr.Location.Map.GetNpc(id)))//todo: change to what is visible
                throw new ModerateViolation("Player attempted to select an invalid NPC.");

            chr.NpcSession.Start(npc, chr);
            return false;
        }
    }
}