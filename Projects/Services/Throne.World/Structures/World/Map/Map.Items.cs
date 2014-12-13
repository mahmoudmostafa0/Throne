using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.Shared;
using Throne.Shared.Threading.Actors;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.World
{
    /// <summary>
    ///     Map operations for user items.
    /// </summary>
    public partial class Map
    {
        private readonly ConcurrentDictionary<UInt32, ActorTimer> _itemTimers;
        private readonly Dictionary<UInt32, Item> _items;
        private Object _itemsRWL;

        public Object ItemReadWrite
        {
            get
            {
                if (_itemsRWL == null) Interlocked.CompareExchange<Object>(ref _itemsRWL, new Object(), null);
                return _itemsRWL;
            }
        }

        public void AddItem(Item itm)
        {
            lock (ItemReadWrite)
            {
                _items[itm.Guid] = itm;
                _itemTimers[itm.Guid] = new ActorTimer(WorldManager.Instance, () => RemoveItem(itm),
                    TimeSpan.FromSeconds(20));
            }

            using (var pkt = new MapItemInformation(itm))
                foreach (Character user in GetVisibleUsers(itm))
                    user.User.Send(pkt);
        }

        public void RemoveItem(Item itm)
        {
            // TODO: fix race condition: timer vs. player picks the item up
            lock (ItemReadWrite)
                if (_items.Remove(itm.Guid))
                {
                    _itemTimers[itm.Guid].Dispose();
                    _itemTimers.Remove(itm.Guid);
                }

            using (var pkt = new MapItemInformation(itm, remove: true))
                foreach (Character user in GetVisibleUsers(itm))
                    user.User.Send(pkt);
        }

        public Item GetItem(UInt32 guid)
        {
            lock (ItemReadWrite)
                return _items.Values.FirstOrDefault(a => a.Guid == guid);
        }

        public List<Item> GetVisibleItems(IWorldObject nearThis)
        {
            var result = new List<Item>();
            Position pos = nearThis.Location.Position;

            lock (ItemReadWrite)
                result.AddRange(
                    _items.Values.Where(
                        itm => itm.Location.Position.InRange(pos, MapSettings.Default.PlayerScreenRange)));

            return result;
        }
    }
}