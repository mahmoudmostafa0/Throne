using System;
using System.Collections;
using Throne.Framework.Utilities;
using Throne.World;
using Throne.World.Records;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;

public sealed class CharacterCreation : DynamicScript
{
	protected override void Load()
	{
		SetName("new_character");
	}

	public override ExecutionResult Execute(IEnumerator param)
	{
		if (!param.MoveNext()) return ExecutionResult.Failed;
		var record = (CharacterRecord) param.Current;

		record.MapID = 1002;
		record.X = 300;
		record.Y = 278;
        record.Create();

		switch (record.CurrentJob)
		{
			case Role.Profession.InternTrojan:
                PrepareTrojan(record);
			break;
			case Role.Profession.InternTaoist:
                PrepareTaoist(record);
			break;
			case Role.Profession.InternArcher:
                PrepareArcher(record);
			break;
			case Role.Profession.InternWarrior:
                PrepareWarrior(record);
			break;
			case Role.Profession.InternNinja:
                PrepareNinja(record);
			break;
			case Role.Profession.InternMonkSaint:
                PrepareMonk(record);
			break;
			case Role.Profession.InternPirate:
                PreparePirate(record);
			break;
			case Role.Profession.InternDragonWarrior:
                PrepareDragonWarrior(record);
			break;

			default:
                return ExecutionResult.Failed;
		}

		record.Update();
		return ExecutionResult.Success;
	}

	public static void PrepareTrojan(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 1, 96, 201, 283);

		ItemManager.Instance.CreateItemRecord(characterRecord, 410401, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PrepareArcher(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 1, 96, 201, 283);

		ItemManager.Instance.CreateItemRecord(characterRecord, 500301, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 1050000, 0, 0, 0, Item.Positions.LeftHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PrepareWarrior(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 1, 96, 201, 283);

		ItemManager.Instance.CreateItemRecord(characterRecord, 410401, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PrepareTaoist(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 1, 96, 201, 283);

		ItemManager.Instance.CreateItemRecord(characterRecord, 421301, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
	}

	public static void PrepareNinja(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 103, 107, 291, 295);

		ItemManager.Instance.CreateItemRecord(characterRecord, 601301, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 511301, 0, 0, 0, Item.Positions.Inventory);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PrepareMonk(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 109, 113, 300, 304);

		ItemManager.Instance.CreateItemRecord(characterRecord, 610301, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132004, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PreparePirate(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 154, 163, 345, 354);

		ItemManager.Instance.CreateItemRecord(characterRecord, 611301, 0, 0, 0, Item.Positions.RightHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 612301, 0, 0, 0, Item.Positions.LeftHand);
		ItemManager.Instance.CreateItemRecord(characterRecord, 132024, 0, 0, 0, Item.Positions.Armor);
		ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
	}

	public static void PrepareDragonWarrior(CharacterRecord characterRecord)
	{
		SetModelAvatar(characterRecord, 164, 173, 355, 364);
		
        ItemManager.Instance.CreateItemRecord(characterRecord, 617301, 0, 0, 0, Item.Positions.RightHand);
        ItemManager.Instance.CreateItemRecord(characterRecord, 138314, 0, 0, 0, Item.Positions.Armor);
        ItemManager.Instance.CreateItemRecord(characterRecord, 150004, 0, 0, 0, Item.Positions.Inventory);
    }

    public static void SetModelAvatar(CharacterRecord record, params Int32[] bounds)
    {
        Random rnd = RandomProvider.Get();
        var model = new Role.Model(record.Look);
        model.Avatar = model.Sex == Role.Model.SexType.Male
            ? rnd.Next(bounds[0], bounds[1])
            : rnd.Next(bounds[2], bounds[3]);
        record.Look = model.Id;
    }
}