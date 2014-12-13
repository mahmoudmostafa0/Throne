using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Throne.Shared;
using Throne.Shared.Exceptions;
using Throne.Shared.Logging;
using Throne.Shared.Threading.Actors;
using Throne.World.Database.Records;
using Throne.World.Properties.Settings;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.World
{
    /// <summary>
    ///     Throne's implementations for maps.
    /// </summary>
    public partial class Map
    {
        public readonly String DataMapPath;
        public readonly Dictionary<Int32, Location> PortalDestination;
        private readonly MapInfoRecord _record;
        public UInt32 Instance;
        public LogProxy Log;

        private Cell[,] _cells;

        public Location _reviveLocation;
        private MapScript _script;

        public Map(MapInfoRecord record)
        {
            _record = record;
            Log = new LogProxy("Region {0}".Interpolate(_record.MapId));

            DataMapPath = record.DataMapPath;

            PortalDestination = new Dictionary<Int32, Location>();

            Load(record.MapId);

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

        public Size Boundaries { get; private set; }

        public MapAttribute Attributes
        {
            get { return _record.Attributes; }
        }

        public uint Document
        {
            get { return _record.DocumentId; }
        }

        public Cell this[int x, int y]
        {
            get { return _cells[x, y]; }
            set { _cells[x, y] = value; }
        }

        public Boolean this[MapAttribute flag]
        {
            get { return (_record.Attributes & flag) == flag; }
            set
            {
                if (value)
                    _record.Attributes |= flag;
                else
                    _record.Attributes &= ~flag;

                _record.Update();
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

        public Int16 GetCellAltitude(Position pos)
        {
            return GetCell(pos).Argument;
        }

        public Cell GetCell(Position pos)
        {
            return this[pos.X, pos.Y];
        }

        private void Load(UInt32 mapId)
        {
            try
            {
                if (!File.Exists(DataMapPath))
                {
                    Log.Error(StrRes.SMSG_MapDataNotFound);
                    return;
                }

                using (var BR = new BinaryReader(File.OpenRead(DataMapPath)))
                {
                    BR.BaseStream.Seek(268, SeekOrigin.Begin); //???

                    int width = BR.ReadInt32();
                    int height = BR.ReadInt32();
                    Boundaries = new Size(width, height);

                    #region Construct base cells

                    _cells = new Cell[width, height];
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            CellType baseType = BR.ReadInt16() == 0 ? CellType.Open : CellType.Terrain;
                            BR.BaseStream.Seek(sizeof (short), SeekOrigin.Current); // tile flag
                            short cellHeight = BR.ReadInt16();

                            this[x, y] = new Cell(baseType, cellHeight);
                        }
                        BR.BaseStream.Seek(4, SeekOrigin.Current);
                    }

                    #endregion

                    #region Override cells for portals

                    int totalPortals = BR.ReadInt32();
                    for (int index = 0; index < totalPortals; index++)
                    {
                        int portalX = BR.ReadInt32() - 1;
                        int portalY = BR.ReadInt32() - 1;
                        int destinationId = BR.ReadInt32();

                        //thanks Fang for the idea of unavoidable portals
                        for (int x = 0; x < 3; x++)
                            for (int y = 0; y < 3; y++)
                                if (portalY + y < height && portalX + x < width)
                                    this[portalX + x, portalY + y].AddFlag(CellType.Portal)
                                        .SetArgument((short) destinationId);
                    }

                    #endregion

                    #region Override cells for objects

                    int amountOfScenery = BR.ReadInt32();
                    for (int index = 0; index < amountOfScenery; index++)
                    {
                        var typeOfScenery = (MapObjectType) BR.ReadInt32();
                        switch (typeOfScenery)
                        {
                            case MapObjectType.Scenery:
                            {
                                string fileName = Encoding.ASCII.GetString(BR.ReadBytes(260));
                                fileName = Environment.CurrentDirectory + "\\" + WorldManager.SystemRoot + "\\" +
                                           fileName.Remove(fileName.IndexOf('\0'));
                                var location = new Point(BR.ReadInt32(), BR.ReadInt32());

                                if (!location.X.IsBetween(0, Boundaries.Width) ||
                                    !location.Y.IsBetween(0, Boundaries.Height))
                                {
                                    Log.Error(StrRes.SMSG_SceneryNotLoaded);
                                    continue;
                                }

                                if (!File.Exists(fileName))
                                {
                                    Log.Error(StrRes.SMSG_SceneryNotFound.Interpolate(fileName));
                                    continue;
                                }

                                using (var sceneReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(fileName)))
                                    )
                                {
                                    for (Int32 amountOfParts = sceneReader.ReadInt32(), part = 0;
                                        part < amountOfParts;
                                        part++)
                                    {
                                        sceneReader.BaseStream.Seek(160, SeekOrigin.Current); //ani
                                        sceneReader.BaseStream.Seek(160, SeekOrigin.Current); //tga
                                        sceneReader.BaseStream.Seek(sizeof (int), SeekOrigin.Current);
                                        //math.abs offset X
                                        sceneReader.BaseStream.Seek(sizeof (int), SeekOrigin.Current);
                                        //math.abs offset Y
                                        sceneReader.BaseStream.Seek(sizeof (int), SeekOrigin.Current); //interval
                                        var size = new Size(sceneReader.ReadInt32(), sceneReader.ReadInt32());
                                        sceneReader.BaseStream.Seek(sizeof (int), SeekOrigin.Current); //thickness
                                        var startPosition = new Point(sceneReader.ReadInt32(), sceneReader.ReadInt32());
                                        sceneReader.BaseStream.Seek(4, SeekOrigin.Current); //pad

                                        for (int y = 0; y < size.Height; y++)
                                            for (int x = 0; x < size.Width; x++)
                                            {
                                                var point = new Point(location.X + startPosition.X - x,
                                                    location.Y + startPosition.Y - y);
                                                bool accessable = sceneReader.ReadInt32() == 0;
                                                if (!accessable)
                                                    this[point.X, point.Y]
                                                        .RemoveFlag(CellType.Open)
                                                        .AddFlag(CellType.Terrain);

                                                sceneReader.BaseStream.Seek(8, SeekOrigin.Current); //junk?
                                            }
                                    }
                                    break;
                                }
                            }
                            case MapObjectType.DDSOverlay:
                                BR.BaseStream.Seek(260, SeekOrigin.Current); //ani file for tga
                                BR.BaseStream.Seek(128, SeekOrigin.Current); //tga file
                                BR.BaseStream.Seek(sizeof (int)*2, SeekOrigin.Current); //position
                                BR.BaseStream.Seek(sizeof (int)*5, SeekOrigin.Current); //arguments/traps?
                                break;
                            case MapObjectType.Effect:
                                BR.BaseStream.Seek(64, SeekOrigin.Current); // effect file
                                BR.BaseStream.Seek(sizeof (int)*2, SeekOrigin.Current); // location
                                break;
                            case MapObjectType.Sound:
                                BR.BaseStream.Seek(260, SeekOrigin.Current); //sound file
                                BR.BaseStream.Seek(sizeof (int)*2, SeekOrigin.Current); //position
                                BR.BaseStream.Seek(sizeof (int), SeekOrigin.Current); //duration
                                BR.BaseStream.Seek(sizeof (int), SeekOrigin.Current); //interval
                                break;
                        }
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                ExceptionManager.RegisterException(e);
                LogManager.Debug(StrRes.SMSG_MapNotLoaded.Interpolate(mapId));
            }
        }
    }
}