using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendAssets(Character chr)
        {
            chr.User.Send(this);
        }
    }
}