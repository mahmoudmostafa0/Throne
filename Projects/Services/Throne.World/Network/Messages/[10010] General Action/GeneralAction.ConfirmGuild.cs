using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendGuild(Character @c)
        {
            @c.User.Send(this);
        }
    }
}