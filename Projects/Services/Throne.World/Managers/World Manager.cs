using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.World.Database.Records;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects;
using Throne.World.Structures.World;

namespace Throne.World
{
    public sealed class WorldManager : SingletonActor<WorldManager>
    {
        private WorldManager()
        {
            _static = new ConcurrentDictionary<uint, Map>();
            Log = new LogProxy("WorldManager");
        }
        public const string SystemRoot = "system/";
        private const string MapRoot = SystemRoot + "map/";
        private const string MapIndexPath = MapRoot + "GameMap.dat";
        public readonly LogProxy Log;

        private readonly ConcurrentDictionary<UInt32, Map> _static;

        public void InitDb()
        {
            Log.Info(StrRes.SMSG_BuildMapDb);

            if (!File.Exists(MapIndexPath))
            {
                Log.Error(StrRes.SMSG_MapIndexNotFound, MapIndexPath);
                return;
            }

            _static.Clear();

            using (var FS = new FileStream(MapIndexPath, FileMode.Open))
            using (var BR = new BinaryReader(FS))
            {
                int added = 0;
                int toDo = BR.ReadInt32();

                for (int done = 0; done < toDo; done++)
                {
                    uint mapId = BR.ReadUInt32();
                    string mapPath = SystemRoot + Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                    uint puzzleId = BR.ReadUInt32();

                    if (mapPath.EndsWith(".7z"))
                        mapPath = Path.ChangeExtension(mapPath, ".dmap");

                    if (!File.Exists(mapPath))
                    {
                        Log.Warn(StrRes.SMSG_MapFileNotFound, mapPath);
                        continue;
                    }

                    if (_static.ContainsKey(mapId))
                    {
                        Log.Warn(StrRes.SMSG_NonUniqueMap, mapId);
                        continue;
                    }

                    var newMap = new MapInfoRecord
                    {
                        MapId = mapId,
                        DocumentId = mapId,
                        DataMapPath = mapPath,
                        EnvironmentColor = "White",
                        SpawnMapId = 1002,
                        SpawnPointX = 300,
                        SpawnPointY = 278,
                        Attributes = MapAttribute.PkField | MapAttribute.BoothEnable
                    };
                    newMap.Create();

                    _static.TryAdd(mapId, new Map(newMap));
                    added++;

                    Log.Progress(done, toDo);
                }

                Log.Info(StrRes.SMSG_MapBuildDone, added, toDo);
            }
        }

        public Map Retrieve(UInt32 StaticId, UInt32 InstanceNum = 0)
        {
            Map map;

            //try to get map from memory
            if (_static.TryGetValue(StaticId, out map)) return map;

            //map was not in memory, load it
            var record = Instance.LoadSingle(StaticId);
            if (record != null) // was the map in the database?
                _static.TryAdd(StaticId, map = new Map(record));

            if (!map)
                Log.Error(StrRes.SMSG_NonsuchMap, StaticId);

            return map;
        }

        public IEnumerable<MapInfoRecord> LoadAll()
        {
            return WorldServer.Instance.WorldDbContext.FindAll<MapInfoRecord>().ToList();
        }

        public MapInfoRecord LoadSingle(UInt32 Id)
        {
            return
                WorldServer.Instance.WorldDbContext.Find<MapInfoRecord>(record => record.MapId == Id).SingleOrDefault();
        }

        public Int32 Count
        {
            get { return _static.Count; }
        }

        public Character GetCharacter(String name)
        {
            return _static.Values.Select(region => region.GetUser(name)).FirstOrDefault(creature => creature != null);
        }
    }
}