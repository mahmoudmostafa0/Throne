using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.Shared;
using Throne.Shared.Logging;
using Throne.Shared.Threading;
using Throne.World.Database.Records.Implementations;
using Throne.World.Records;
using Throne.World.Structures.Objects;

namespace Throne.World
{
    public sealed class ItemManager : SingletonActor<ItemManager>
    {
        private readonly LogProxy _log;
        private ItemCollection _collection;
        private SerialGenerator serialGenerator;

        private ItemManager()
        {
            _log = new LogProxy("ItemManager");
            _collection = new ItemCollection();

            SerialGeneratorManager.Instance.PostAsync(
                sg =>
                    serialGenerator =
                        sg.GetSerialGenerator(typeof (ItemRecord).Name, WorldObject.ItemIdMin,
                            WorldObject.ItemIdMax));
        }

        public Item CreateItem(Character chr, Int32 type)
        {
            var record = new ItemRecord
            {
                Guid = serialGenerator.Next(),
                Owner = chr.Record,
                Type = type
            };
            record.Create();

            var item = new Item(record);
            chr.AddInventoryItem(item);
            return item;
        }

        private sealed class ItemCollection : Dictionary<UInt32, Item>
        {
            private readonly ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim();
            private LogProxy _log = new LogProxy("ItemCollection");

            public new IReadOnlyList<Item> Values
            {
                get { return base.Values.ToArray(); }
            }

            public Boolean Contains(Item itm)
            {
                return Contains(itm.ID);
            }

            public Boolean Contains(UInt32 chrId)
            {
                Boolean contained;

                _rwls.EnterWriteLock();
                try
                {
                    contained = ContainsKey(chrId);
                }
                finally
                {
                    _rwls.ExitWriteLock();
                }

                return contained;
            }

            public void Add(Item itm)
            {
                _rwls.EnterWriteLock();
                try
                {
                    this[itm.ID] = itm;
                }
                finally
                {
                    _rwls.ExitWriteLock();
                }
            }

            public void Remove(Item itm)
            {
                Remove(itm.ID);
            }

            public new void Remove(UInt32 chrId)
            {
                _rwls.EnterWriteLock();
                try
                {
                    Remove(chrId);
                }
                finally
                {
                    _rwls.ExitWriteLock();
                }
            }

            public override string ToString()
            {
                return "{0} cached items.".Interpolate(Count);
            }
        }
    }
}