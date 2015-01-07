using System;

namespace Throne.World.Structures.World
{
    [Flags]
    public enum MapAttribute : long
    {
        Normal = 0x0,
        PkField = 0x1,
        ChangeMapDisable = 0x2,
        RecordDisable = 0x4,
        PkDisable = 0x8,
        BoothEnable = 0x10,
        TeamDisable = 0x20,
        TeleportDisable = 0x40,
        GuildMap = 0x80,
        PrisonMap = 0x100,
        FlyingDisabled = 0x200,
        House = 0x400,
        MineField = 0x800,
        NewbieSafetyArea = 0x1000,
        RebornNowEnable = 0x2000,
        NewbieProtect = 0x4000,
        TrainingDisable = 0x8000,
        MountMap = 0x2000000,
        GuildMapActive = 0x4000000,
        GuildCTFMap = 0x10000000,
        TakeItemsInAttack = 0x80000000000
    }

    [Flags]
    public enum CellType : byte
    {
        None = 0x0,
        Open = 0x1,
        Portal = 0x2, 
        Item = 0x4,
        Npc = 0x8,
        Monster = 0x10,
        Terrain = 0x20
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