using System;
using Throne.Framework.Network.Transmission;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public class RoleInfo : WorldPacket
    {
        public const Int32 MIN_SIZE = 267;

        public RoleInfo(Role role)
            : base(PacketTypes.RoleInfo, MIN_SIZE + role.Name.Length)
        {
            if (role is Character)
                AddCharacterInfo(role as Character);
        }

        private void AddCharacterInfo(Character c)
        {
            WriteInt(Environment.TickCount);
            WriteInt(c.Look);
            WriteUInt(c.ID);
            WriteInt(0); // guild id
            WriteInt(0); // guild rank
            SeekForward(sizeof(short)); // unknown
            WriteArray(c.PublicStates);
            WriteShort((short) c.Appearance); //Appearance type
            WriteInt(c.GetGearType(Item.Positions.Headgear));
            WriteInt(c.GetGearType(Item.Positions.Garment));
            WriteInt(c.GetGearType(Item.Positions.Armor));
            WriteInt(c.GetGearType(Item.Positions.RightHand));
            WriteInt(c.GetGearType(Item.Positions.LeftHand));
            WriteInt(c.GetGearType(Item.Positions.RightWeaponAccessory));
            WriteInt(c.GetGearType(Item.Positions.LeftWeaponAccessory));
            WriteInt(c.GetGearType(Item.Positions.Mount));
            WriteInt(c.GetGearType(Item.Positions.MountArmor));
            SeekForward(sizeof(short) * 2); // unknown
            WriteUInt(0); // creature health
            SeekForward(sizeof(short)); // unknown
            WriteByte(0); // creature level
            SeekForward(sizeof(short) + 1); // unknown
            WriteShort(c.Location.Position.X);
            WriteShort(c.Location.Position.Y);
            WriteShort(c.Hairstyle);
            WriteByte((byte)c.Direction);
            WriteShort(0); // pose
            SeekForward(sizeof(short) * 2 + 1); // unknown + padding, todo: is this a place for strings?
            WriteByte(2); // rebirth
            WriteByte(c.Level);
            SeekForward(sizeof (bool)); // unknown
            WriteBoolean(false); // show equipment window
            WriteBoolean(c.Away); // afk status
            WriteInt(0); // extended battlepower
            SeekForward(sizeof(int) * 4); // race items? sxm
            WriteInt(0); // goodwill rank (flowers-female, gifts-male)
            WriteInt(0); // nobility rank
            WriteShort(0); // armor color
            WriteShort(0); // shield color-- cannot be set for weapons, bugs the weapon if an environment color is set.
            WriteShort(0); // headgear color
            WriteInt(0); // quiz points
            WriteUShort(0); // mount composition
            WriteInt(0); // mount color, argb?..
            WriteInt(0); // available enlightenment points
            WriteInt(0); // merit status
            SeekForward(2); // padding
            SeekForward(sizeof(short)); // unknown
            SeekForward(sizeof(int)); // unknown
            WriteInt(0); // clan id
            WriteInt(0); // clan rank
            SeekForward(sizeof(int) * 2); // unknown
            WriteShort(0); // title
            SeekForward(sizeof(int)); // unknown
            WriteByte(0); // sizeadd
            SeekForward(sizeof(int)); // unknown
            WriteBoolean(false); // fighting in arena
            SeekForward(sizeof(bool) * 2); // unknown
            SeekForward(sizeof(bool)); // world boss
            WriteUInt(0u); // headgear overlay
            WriteUInt(0u); // armor overlay
            WriteUInt(0u); // off-hand overlay
            WriteUInt(0u); // main-hand overlay
            WriteByte(0); // subclass
            WriteShort(c.PreviousJob);
            WriteShort(c.AncestorJob);
            WriteShort(c.CurrentJob);
            WriteShort(0); // country
            WriteInt(0); // transform power, uses battlepower
            WriteInt(0); // transform color, uses battlepower
            WriteByte(4); // jianghu talent count
            WriteBoolean(true); // jianghu active
            SeekForward(1 + sizeof(short)); // pad + unknown
            WriteByte(0); // entourage size
            SeekForward(sizeof(ushort)); // associate id
            SeekForward(sizeof(uint)); // the guid of the associate's entourage
            WriteStrings(c.Name);
        }
    }

}