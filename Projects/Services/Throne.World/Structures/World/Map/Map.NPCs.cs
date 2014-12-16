using System;
using System.Collections.Generic;
using System.Threading;
using Throne.World.Structures.Objects;

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
    }
}