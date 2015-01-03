using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void SendAssets(Character chr)
        {
            chr.User.Send(chr.ItemStream.Join(this));
        }
    }
}