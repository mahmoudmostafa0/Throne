using System;
using Throne.Shared.Math;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void SendLocation(Character character)
        {
            var loc = character.Location;

            ProcessTimestamp = Environment.TickCount;
            ObjectId = loc.Map.Id;
            Argument = loc.Map.Document;
            this[0] = MathUtils.BitFold32(loc.Position.X, loc.Position.Y);

            character.User.Send(this);
        }
    }
}