using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Travel;

public sealed class Surgeon : NpcScript
{
    public override void Load()
    {
        SetDisplayName("Surgeon");
        SetFace(360);
        SetMesh(25);
        SetIdentity(23545);
        SetFacing(Orientation.Southwest);
        SetLocation(new Location(1002, new Position(332, 230)));
    }
}