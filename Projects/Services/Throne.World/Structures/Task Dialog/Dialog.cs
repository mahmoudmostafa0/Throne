using System;
using System.Collections.Generic;
using Throne.Framework.Network.Transmission.Stream;
using Throne.World.Network;

namespace Throne.World.Structures
{
    public class Dialog
    {
        public Dialog()
        {
            Children = new List<Dialog>();
        }

        public Dialog(params Dialog[] elements) : this()
        {
            Children.AddRange(elements);
        }

        public List<Dialog> Children { get; private set; }

        public Dialog Join(params Dialog[] elements)
        {
            Children.AddRange(elements);
            return this;
        }

        public Stream MakeStream()
        {
            var streamBuilder = new Stream();
            foreach (var child in Children)
                streamBuilder.Join(child.MakePacket());
            return streamBuilder;
        }

        protected virtual WorldPacket MakePacket()
        {
            throw new InvalidOperationException();
        }
    }
}