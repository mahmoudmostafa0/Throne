using Throne.World.Network.Messages;

namespace Throne.World.Structures.Travel
{
    public struct Jump
    {
        public GeneralAction Info;
        public Position Destination;
        public Position ReportedCurrentPosition;
    }
}
