using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Throne.Shared;
using Throne.Shared.Threading.Actors;
using Throne.World.Database.Records;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.World
{
    /// <summary>
    /// Throne's implementations for maps.
    /// </summary>
    public sealed partial class Map : MapBase
    {
        public uint Instance;

        public Location _reviveLocation;
        private MapScript _script;

        public Map(MapInfoRecord record)
            : base(record)
        {
            _reviveLocation = new Location(record.SpawnMapId, new Position(record.SpawnPointX, record.SpawnPointY));
            _users = new Dictionary<UInt32, Character>();
            _items = new Dictionary<uint, Item>();
            _itemTimers = new ConcurrentDictionary<UInt32, ActorTimer>();
        }

        public uint Id
        {
            get { return _record.MapId; }
        }

        public Location ReviveLocation
        {
            get { return _reviveLocation; }
            private set
            {
                _reviveLocation = value;
                _record.SpawnMapId = value.Map.Id;
                _record.SpawnPointX = value.Position.X;
                _record.SpawnPointY = value.Position.Y;
                _record.Update();
            }
        }

        public MapScript Script
        {
            get
            {
                if (!_script)
                    _script = ScriptManager.Instance.GetMapScript(this);
                return _script;
            }
        }

        public override string ToString()
        {
            return "{0} (Id: {1}{2}, Players: {3}".Interpolate((MapIds) Id, Id, Instance != 0 ? ", #" + Instance : "",
                _users.Count);
        }

        public static implicit operator Boolean(Map map)
        {
            return map != null;
        }
    }
}