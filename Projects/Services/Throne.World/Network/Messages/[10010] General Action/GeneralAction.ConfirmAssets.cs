using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void SendAssets()
        {
            Character.User.Send(Character.ItemStream.Join(this));
        }
    }
}