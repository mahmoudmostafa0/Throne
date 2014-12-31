using Throne.World.Network.Messages;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Travel;

public sealed class AuctionManager : NpcScript
{
    public override void Load()
    {
        SetLongName("Auction Manager");
        SetIdentity(1650);
        SetFacing(Orientation.Southeast);
        SetFace(58);
        SetMesh(1650);
        SetLocation(new Location(1002, new Position(290, 228)));
        SetType(NpcInformation.Types.RoleClan);
    }
}