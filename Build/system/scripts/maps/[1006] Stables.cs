using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

public sealed class Stable : MapScript
{
    public override bool Init()
    {
        SetMap(Id: 1006);

        return base.Init();
    }

    public override void OnEnter(Character chr)
    {
        chr.User.Send("Stables");
    }

    public override void LoadWarps()
    {
        AddWarp(index: 0, destination: new Location(1002, 255, 290));
    }
}