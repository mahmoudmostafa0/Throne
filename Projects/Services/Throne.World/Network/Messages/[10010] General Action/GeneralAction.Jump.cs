using System;
using Throne.Framework.Math;
using Throne.World.Structures.Travel;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void Jump()
        {
            var invalidated = false;
            if (!Character.ID.Equals(ObjectId))
                invalidated = InvalidValue(Character.User, "ObjectId", ObjectId, Character.ID);
            if (!Character.Location.MapId.Equals((UInt32)ArgumentEx2))
                invalidated = InvalidValue(Character.User, "ArgumentEx2(MapID)", ArgumentEx2);

            if (invalidated) return;

            Int32 x, y;
            MathUtils.BitUnfold32((Int32)Argument, out x, out y);
            var destination = new Position((Int16) x, (Int16) y);

            var reportedLocation = new Position(ShortArgumentEx1, ShortArgumentEx2);


            Character.Jump(new Jump { Info = this, ReportedCurrentPosition = reportedLocation, Destination = destination });
        }
    }
}