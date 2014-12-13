using System;
using System.Reflection;
using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Transmission;

namespace Throne.Shared.Network.Handling
{
    public sealed class PacketHandler<T>
        where T : Packet
    {
        public PacketHandler(ConstructorInfo constructor, Enum typeId, Type permission)
        {
            TypeId = typeId;
            Permission = permission;
            Constructor = constructor;
        }

        public ConstructorInfo Constructor { get; private set; }

        public Type Permission { get; private set; }

        public Enum TypeId { get; private set; }

        public void Invoke(IClient client, params object[] parameters)
        {
            var processor = (T) Constructor.Invoke(parameters);

            if (processor.Read(client))
                processor.Handle(client);

            processor.Dispose();
        }
    }
}