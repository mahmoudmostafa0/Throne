using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendGuild()
        {
            Character.User.Send(this);
        }
    }
}