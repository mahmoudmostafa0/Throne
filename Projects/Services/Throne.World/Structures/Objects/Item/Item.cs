using System;
using System.Collections.Generic;
using Throne.Framework.Network.Transmission.Stream;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Records;
using Throne.World.Scripting.Scripts;

namespace Throne.World.Structures.Objects
{
    public partial class Item : WorldObject
    {
        public readonly ItemRecord Record;

        public Item(ItemRecord record)
            : base(record.Guid)
        {
            Record = record;
        }

        #region Scripting 

        //TODO: Move this to the item information class when implemented.

        private ItemScript _script;

        public ItemScript Script
        {
            get
            {
                if (!_script)
                    _script = ScriptManager.Instance.GetItemScript(this);
                return _script;
            }
        }

        #endregion

        public UInt32 Guid
        {
            get { return Record.Guid; }
        }

        public CharacterRecord OwnerInfo
        {
            get { return Record.Owner; }
            set
            {
                Record.Owner = value;
                Record.Update();
            }
        }

        public Int32 Type
        {
            get { return Record.Type; }
        }

        public Positions Position
        {
            get { return Record.Position; }
            set
            {
                Record.Position = value;
                Record.Update();
            }
        }

        public static Stream ToStream(IEnumerable<Item> toSend)
        {
            var _stream = new Stream();
            foreach (Item item in toSend)
                _stream.Join(item);
            return _stream;
        }

        public static implicit operator Boolean(Item item)
        {
            return item != null;
        }

        public static implicit operator Byte[](Item toAdd)
        {
            return new ItemInformation(toAdd);
        }

        public override void SpawnFor(WorldClient observer)
        {
            using (var pkt = new MapItemInformation(this))
                observer.Send(pkt);
        }

        public override void DespawnFor(WorldClient observer)
        {
            using (var pkt = new MapItemInformation(this, true))
                observer.Send(pkt);
        }
    }
}