using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public sealed class UserCityInfo : WorldPacket
    {
        public UserCityInfo() : base(PacketTypes.UserCityInfo, 268)
        {
            
        }
    }
}
