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
            if (!Character.Location.MapId.Equals((UInt32)this[1]))
                invalidated = InvalidValue(Character.User, "ArgumentEx_1(MapID)", this[1]);

            if (invalidated) return;

            Int32 x, y;
            MathUtils.BitUnfold32((Int32)Argument, out x, out y);
            var destination = new Position((Int16) x, (Int16) y);

            MathUtils.BitUnfold32((Int32)this[0], out x, out y);
            var reported_location = new Position((Int16) x, (Int16) y);


            Character.Jump(new Jump { Info = this, ReportedCurrentPosition = reported_location, Destination = destination });
        }
    }
}