
using Throne.Shared.Math;
using Throne.World.Structures.Travel;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public GeneralAction Teleport(Location destination)
        {
            Direction = Character.Direction;
            Argument = destination.Map.Id;
            this[0] = MathUtils.BitFold32(destination.Position.X, destination.Position.Y);
            return this;
        }
    }
}
