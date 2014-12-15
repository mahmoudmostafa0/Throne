using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

public sealed class TwinCity : MapScript
{
	public override bool Init()
	{
		SetMap(Id: 1002);

		return base.Init();
	}

	public override void OnEnter(Character chr)
	{
		chr.User.Send("Twin City");
	}

	public override void LoadWarps()
	{
        AddWarp(index: 0, destination: new Location(1004, 51, 71)); // promo center
        AddWarp(index: 1, destination: new Location(1006, 39, 30)); // stables
        //AddWarp(index: 2, destination: new Location(0, 0, 0)); // phoenix castle, x810 y522
        //AddWarp(index: 4, destination: new Location(0, 0, 0)); // bird island, x527 y327
        //AddWarp(index: 5, destination: new Location(0, 0, 0)); // ape city, x522 y810
        //AddWarp(index: 6, destination: new Location(0, 0, 0)); // desert city, x66 y439
	}
}