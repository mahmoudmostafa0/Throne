using Throne.Framework.Math;
using Throne.World.Structures.Travel;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public GeneralAction Teleport(Location destination)
        {
            Direction = Character.Direction;
            Argument = destination.Map.Id;
            ArgumentEx1 = MathUtils.BitFold32(destination.Position.X, destination.Position.Y);
            return this;
        }

        //i tried
        //[StructLayout(LayoutKind.Explicit, Size = 32)]
        //public struct TeleportStructure : IMemberwiseSerializable<Byte[]>
        //{
        //    [FieldOffset(12)] private readonly Int64 _destinationMapId;
        //    [FieldOffset(24)] private readonly Int16 _actionType;
        //    [FieldOffset(28)] private readonly Int16 _destinationX;
        //    [FieldOffset(30)] private readonly Int16 _destinationY;

        //    public TeleportStructure(Location destination)
        //    {
        //        _actionType = (Int16) ActionType.Teleport;
        //        _destinationMapId = destination.MapId;
        //        _destinationX = destination.Position.X;
        //        _destinationY = destination.Position.Y;
        //    }

        //    Byte[] IMemberwiseSerializable<Byte[]>.Serialize()
        //    {
        //        return this.GetBytes();
        //    }
        //}
    }
}