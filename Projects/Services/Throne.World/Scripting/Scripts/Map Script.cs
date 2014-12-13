using System;
using System.Collections.Generic;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;
using Throne.World.Structures.World;

namespace Throne.World.Scripting.Scripts
{
    public abstract class MapScript : ScriptBase
    {
        public Boolean Instance;
        public UInt32 MapId;
        public Dictionary<Int32, Location> Warps;

        public Map Map
        {
            get { return WorldManager.Instance.Retrieve(MapId); }
        }

        public virtual void OnEnter(Character chr)
        {
        }

        public virtual void OnExit(Character chr)
        {
        }

        public virtual void LoadWarps()
        {
        }


        public override bool Init()
        {
            if (!Map)
            {
                Log.Error(StrRes.SMSG_NonsuchMap, MapId);
                return false;
            }

            Warps = new Dictionary<int, Location>();

            Load();
            LoadWarps();

            ScriptManager.Instance.AddMapScript(this);
            return true;
        }


        public void AddWarp(Int32 index, Location destination)
        {
            Warps.Add(index, destination);
        }

        public void SetMap(UInt32 Id)
        {
            MapId = Id;
        }

        public void InstanceScript()
        {
            Instance = true;
        }
    }

    public sealed class DummyMapScript : MapScript
    {
        public DummyMapScript(Map map)
        {
            map.Log.Info(StrRes.SMSG_NoScript);
        }
    }
}