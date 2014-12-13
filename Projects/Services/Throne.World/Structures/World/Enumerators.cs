using System;

namespace Throne.World.Structures.World
{
    [Flags]
    public enum MapAttribute : long
    {
        Normal = 0,
        PkField = 1 << 0,
        ChangeMapDisable = 1 << 1,
        RecordDisable = 1 << 2,
        PkDisable = 1 << 3,
        BoothEnable = 1 << 4,
        TeamDisable = 1 << 5,
        TeleportDisable = 1 << 6,
        GuildMap = 1 << 7,
        PrisonMap = 1 << 8,
        WingDisable = 1 << 9,
        Family = 1 << 10,
        MineField = 1 << 11,
        CallNewbieDisable = 1 << 12,
        RebornNowEnable = 1 << 13,
        NewbieProtect = 1 << 14,
        TrainingDisable = 1 << 15,
        MountMap = 1 << 25,
        GuildMapActive = 1 << 26,
        GuildCTFMap = 1 << 28,
        TakeItemsInAttack = 1 << 43
    }

    [Flags]
    public enum CellType : byte
    {
        None,
        Open,
        Portal,
        Item,
        Npc,
        Monster,
        Terrain
    }

    public enum MapObjectType
    {
        Scenery = 1,
        DDSOverlay = 4,
        Effect = 10,
        Sound = 15
    }

    public enum MapIds : uint
    {
        TwinCity = 1002,
        BeginnersVillage = 1010
    }
}