using System;
using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.World.Database.Records.Implementations;
using Throne.World.Records;
using Throne.World.Structures.Objects;

namespace Throne.World
{
    public sealed class ItemManager : SingletonActor<ItemManager>
    {
        private readonly LogProxy _log;
        private readonly SerialGenerator _serialGenerator;

        private ItemManager()
        {
            _log = new LogProxy("ItemManager");

            SerialGeneratorManager.Instance.GetGenerator(typeof (ItemRecord).Name, WorldObject.ItemIdMin,
                WorldObject.ItemIdMax, ref _serialGenerator);
        }

        public Item CreateItem(Character chr, Int32 type)
        {
            var record = new ItemRecord
            {
                Guid = _serialGenerator.Next(),
                Owner = chr.Record,
                Type = type
            };
            record.Create();

            var item = new Item(record);
            chr.MoveToInventory(item);
            item.Script.OnCreation(chr, item);

            return item;
        }
    }
}