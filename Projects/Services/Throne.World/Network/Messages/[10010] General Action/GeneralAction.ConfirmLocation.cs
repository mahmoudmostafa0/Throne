using System;
using Throne.Framework.Math;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendLocation()
        {
            var loc = Character.Location;

            ProcessTimestamp = Environment.TickCount;
            ObjectId = loc.Map.Id;
            Argument = loc.Map.Document;
            this[0] = MathUtils.BitFold32(loc.Position.X, loc.Position.Y);

            Character.User.Send(this);
        }
    }
}