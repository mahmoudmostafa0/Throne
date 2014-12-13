using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendProficiencies(Character @c)
        {
            @c.User.Send(this);
        }
    }
}