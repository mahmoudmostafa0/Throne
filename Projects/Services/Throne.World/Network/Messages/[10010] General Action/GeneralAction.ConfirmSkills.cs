namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendSkills()
        {
            Character.User.Send(this);
        }
    }
}