using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Throne.World.Structures.Objects;

namespace Throne.World.Structures.Storage
{
    public class Depository : ItemStorage
    {
        public readonly DepositoryType Type;
        private readonly ConcurrentDictionary<UInt32, Item> _items;
        private readonly Int32 _size;


        public Depository(DepositoryType location, Int32 size)
        {
            _items = new ConcurrentDictionary<uint, Item>();
            _size = size;
            Type = location;
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
            return _items.Count < _size && _items.TryAdd(item.ID, item);
        }

        public override Item Remove(UInt32 guid)
        {
            Item ret;
            _items.TryRemove(guid, out ret);
            return ret;
        }

        /// <summary>
        ///     Creates a network message for the client from the depository information.
        /// </summary>
        /// <param name="toSend"></param>
        /// <returns>Byte array that represents a complete depository packet.</returns>
        public static implicit operator Byte[](Depository toSend)
        {
            throw new NotImplementedException();
        }
    }

    public enum DepositoryType : byte
    {
        None,
        TwinCity,
        PhoenixCastle,
        DesertCity,
        BirdIsland,
        AncientCity,
        Marketplace
    }
}