

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void Away()
        {
            Character.Away = Argument == 1;
        }
    }
}
