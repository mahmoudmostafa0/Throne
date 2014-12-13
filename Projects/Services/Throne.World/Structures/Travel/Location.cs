using System;
using System.Drawing;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using Throne.Shared;
using Throne.World.Database;
using Throne.World.Structures.World;

namespace Throne.World.Structures.Travel
{
    /// <summary>
    ///     Representation of a location in the world.
    /// </summary>
    public class Location
    {
        public readonly UInt32 MapId;
        public Position Position;

        public Location(UInt32 mapId, Int16 x, Int16 y)
            : this(mapId, new Position(x, y))
        {
        }

        public Location(UInt32 mapId, Position point)
        {
            Position = point;
            MapId = mapId;
        }

        public Map Map
        {
            get { return WorldManager.Instance.Retrieve(MapId); }
        }

        public static Location None
        {
            get { return new Location(0, 0, 0); }
        }

        public Location Copy
        {
            get { return new Location(MapId, Position); }
        }

        public static Boolean operator ==(Location loc1, Location loc2)
        {
            var objThis = loc1 as object;
            var objOther = loc2 as object;
            if (objThis == null && objOther == null)
                return true;
            if (objThis == null && objOther != null)
                return false;
            if (objThis != null && objOther == null)
                return false;
            return loc1.MapId == loc2.MapId;
        }

        public static Boolean operator !=(Location loc1, Location loc2)
        {
            return !(loc1 == loc2);
        }

        /// <summary>
        /// Checks if the location is not null and valid.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns>True only if the location is not null, has a valid map id, instance, and position.</returns>
        public static implicit operator Boolean(Location loc)
        {
            return loc != null && WorldManager.Instance.Retrieve(loc.MapId)[loc.Position.X, loc.Position.Y];
        }

        public override Int32 GetHashCode()
        {
            return Position.GetHashCode() ^ (Int32)MapId;
        }

        public override Boolean Equals(object obj)
        {
            return obj is Location && this == (Location)obj;
        }

        public override String ToString()
        {
            var map = Map;
            return "(Location: {0} {1}, {2})".Interpolate(MapId, map.Instance > 0 ? "#" + map.Instance : "", Position);
        }
    }
}