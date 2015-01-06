using Throne.World.Structures.World;

namespace Throne.World.Network.Messages
{
    partial class GeneralAction
    {
        private void SendLocation()
        {
            Map map = Character.Location.Map;
            ObjectId = map.Id;
            Argument = map.Document;
            ShortArgumentEx1 = Character.Location.Position.X;
            ShortArgumentEx2 = Character.Location.Position.Y;
            Character.User.Send(this);
        }
    }
}