using Throne.Framework.Network.Transmission;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public sealed class BattleEffectiveness : WorldPacket
    {
        public BattleEffectiveness(Character chr) : base(PacketTypes.BattleEffectiveness, 15 + 8)
        {
            WriteUInt(chr.ID);
            WriteByte(0); // battlepower
        }
    }
}