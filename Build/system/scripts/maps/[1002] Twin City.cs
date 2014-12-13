using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;

public sealed class TwinCity : MapScript
{
	public override bool Init()
	{
		SetMap(Id: 1002);

		return base.Init();
	}

	public override void OnEnter(Character chr)
	{
		chr.User.Send("Welcome to Twin City");
	}

	public override void LoadWarps()
	{
		//AddWarp(index: 0, destination: new Location(1000, 300, 300)); // promo center
		//AddWarp(index: 1, destination: new Location(0, 0, 0)); // stables
	}
}