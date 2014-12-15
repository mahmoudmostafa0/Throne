using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

public sealed class PromotionCenter : MapScript
{
    public override bool Init()
    {
        SetMap(Id: 1004);

        return base.Init();
    }

    public override void OnEnter(Character chr)
    {
        chr.User.Send("Training Center");
    }

    public override void LoadWarps()
    {
        AddWarp(index: 0, destination: new Location(1002, 275, 290)); // twin city
    }
}