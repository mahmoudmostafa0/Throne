namespace Throne.World.Network.Messages
{
    public partial class ItemAction
    {
        private void PingResponse()
        {
            Character.User.Send(this);
        }
    }
}