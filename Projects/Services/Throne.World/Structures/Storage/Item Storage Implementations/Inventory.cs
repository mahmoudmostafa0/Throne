using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Throne.World.Structures.Objects;

namespace Throne.World.Structures.Storage
{
    public sealed class Inventory : ItemStorage
    {
        private const Int32 SIZE = 40;
        private readonly ConcurrentDictionary<UInt32, Item> _items;

        public Inventory(IEnumerable<Item> payload)
        {
            _items = new ConcurrentDictionary<uint, Item>();

            if (payload == null) return;
            foreach (var item in payload.Where(item => item.Position == Item.Positions.Inventory))
                Add(item);
        }

        public override IEnumerable<Item> Items
        {
            get { return _items.Values as IReadOnlyCollection<Item>; }
        }

        public override Int32 Count
        {
            get { return _items.Count; }
        }

        public override Boolean Add(Item item)
        {
            lock (SyncRoot)
                return _items.Count < SIZE && _items.TryAdd(item.ID, item);
        }

        public override Item Remove(UInt32 guid)
        {
            lock (SyncRoot)
            {
                Item ret;
                _items.TryRemove(guid, out ret);
                return ret;
            }
        }

        public Boolean AdequateSpace(Int32 forCount)
        {
            return Count + forCount <= SIZE;
        }

        public Item this[UInt32 id]
        {
            get { return _items[id]; }
            set { _items[id] = value; }
        }


        /// <summary>
        /// To be used with a <see cref="T:Throne.Shared.Network.Transmission.Stream"/>
        /// </summary>
        /// <param name="inv">Inventory to serialize</param>
        /// <returns></returns>
        public static implicit operator Byte[][](Inventory inv)
        {
            lock (inv.SyncRoot)
                return inv.Items.Select(item => (byte[])item).ToArray();
        }
    }
}