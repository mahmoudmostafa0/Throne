using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.World.Network;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Objects.Actors;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.World
{
    /// <summary>
    ///     Map operations for NPCs.
    /// </summary>
    public partial class Map
    {
        private readonly Dictionary<UInt32, Npc> _npcs;
        private Object _npcsRWL;

        public Object NpcReadWrite
        {
            get
            {
                if (_npcsRWL == null) Interlocked.CompareExchange<Object>(ref _npcsRWL, new Object(), null);
                return _npcsRWL;
            }
        }

        public Boolean AddNpc(Npc npc)
        {
            lock (NpcReadWrite)
                _npcs[npc.ID] = npc;

            using (WorldPacket pkt = npc)
                foreach (Character user in GetVisibleUsers(npc))
                    user.User.Send(pkt);
            return true;
        }

        public void RemoveNpc(Npc npc)
        {
            // TODO: fix race condition: timer vs. player picks the item up
            lock (NpcReadWrite)
                _npcs.Remove(npc.ID);
        }

        public void RemoveAllNpcs()
        {
            lock (NpcReadWrite)
                foreach (Npc npc in _npcs.Values)
                    RemoveNpc(npc);
        }

        public Npc GetNpc(UInt32 guid)
        {
            lock (NpcReadWrite)
                return _npcs[guid];
        }

        public List<Npc> GetVisibleNpcs(IWorldObject nearThis)
        {
            var result = new List<Npc>();
            Position pos = nearThis.Location.Position;

            lock (ItemReadWrite)
                result.AddRange(
                    _npcs.Values.Where(
                        npc => npc.Location.Position.InRange(pos, MapSettings.Default.PlayerScreenRange)));
            //todo: find npc screen range

            return result;
        }
    }
}