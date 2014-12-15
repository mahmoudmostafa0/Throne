
namespace Throne.World.Structures.Objects
{
    public enum RoleState : byte
    {
        None = 0x0,
        Crime = 0,
        Poisoned,
        Invisible,
        Die,
        XpList,
        Dead,
        GroupLeader,
        StayOfAccuracy,
        MagicShield,
        Stigma,
        Ghost,
        Disappear,
        MagicDefense,
        BowDefense,
        RedName,
        BlackName,
        AttackRange,
        ReflectMelee,
        Superman = 18,
        MagicDamage = 20,
        AttackSpeed,
        Invisibility,
        Cyclone,
        GuildCrime,
        ReflectMagic,
        Dodge,
        Fly,
        Intensify,
        Stop,
        Pray,
        FollowPrayer,
        Cursed,
        GodBless,
        TopGuildLeader,
        TopDeputyLeader,
        MonthlyPKChampion,
        WeeklyPKChampion,
        TopWarrior,
        TopTrojan,
        TopArcher,
        TopWaterTaoist,
        TopFireTaoist,
        TopNinja = 43,
        ShurikenVortex = 46,
        FatalStrike,
        Flashy

        //160 Dragon Cyclone
        //154 top dragon
        //153 supreme guild rank 3
        //152 supreme guild rank 2
        //151 supreme guild rank 1
        //150 super cyclone
        //148 auto hunting
        //147 kinetic spark
        //146 blade flurry
        //145 assasin
        //132 faction two icon
        //131 faction one icon
        //130 faction two active
        //129 faction one active
        //128 magic defender
        //126 defending stance
        //125 top 3 pirate
        //124 top 2 pirate
        //123 top 8 pirate
        //122 top pirate
        //119 purple gas?
        //118 flag carrier
        //117 top 3 monk
        //116 top 2 monk
        //115 top 8 monk
        //114 top monk
        //112 oblivion
        //111 soul shackle
    }

    public enum RoleAppearance : short
    {
        Default,
        Garment,
        Artifact,
        Equipment,
        Default_NoAccessories,
        Garment_NoAccessories,
        Artifact_NoAccessories,
        Equipment_NoAccessories
    }
}
