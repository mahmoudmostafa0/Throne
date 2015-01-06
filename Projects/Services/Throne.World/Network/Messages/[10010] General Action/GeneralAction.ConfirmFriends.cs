namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendFriends()
        {
            Character.User.Send(this);
        }
    }
}