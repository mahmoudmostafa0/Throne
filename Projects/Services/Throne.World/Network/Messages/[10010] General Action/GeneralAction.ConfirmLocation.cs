using Throne.Framework.Math;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public GeneralAction ConfirmLocation()
        {
            var loc = Character.Location;

            ObjectId = loc.Map.Id;
            Argument = loc.Map.Document;
            this[0] = MathUtils.BitFold32(loc.Position.X, loc.Position.Y);
            return this;
        }
    }
}