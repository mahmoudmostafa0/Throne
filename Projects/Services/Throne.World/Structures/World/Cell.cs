using System;
using System.Runtime.InteropServices;

namespace Throne.World.Structures.World
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cell
    {
        public CellType Flags;

        /// <summary>
        ///     Holds altitude for valid tiles.
        ///     If the tile is flagged as a portal, this will be the portal's destination ID.
        /// </summary>
        public Int16 Argument;


        public Cell(CellType baseType, Int16 altitude)
        {
            Flags = baseType;
            Argument = altitude;
        }

        public static implicit operator Boolean(Cell cell)
        {
            return cell[CellType.Open];
        }

        public Boolean this[CellType flag]
        {
            get { return (Flags & flag) == flag; }
            set
            {
                if (value)
                    Flags |= flag;
                else
                    Flags &= ~flag;
            }
        }

        public Cell AddFlag(CellType flag)
        {
            this[flag] = true;
            return this;
        }

        public Cell RemoveFlag(CellType flag)
        {
            this[flag] = false;
            return this;
        }

        public Cell SetArgument(Int16 value)
        {
            Argument = value;
            return this;
        }
    }
}