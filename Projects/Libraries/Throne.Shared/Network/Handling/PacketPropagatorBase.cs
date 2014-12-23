using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Throne.Framework.Logging;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Reflection;
using Throne.Framework.Security.Permissions;
using Throne.Framework.Network.Transmission;

namespace Throne.Framework.Network.Handling
{
    public abstract class PacketPropagatorBase<TAttribute, TPacket> : IPacketPropagator
        where TAttribute : PacketHandlerAttribute
        where TPacket : Packet
    {
        private static readonly LogProxy Log = new LogProxy("PacketPropagatorBase");

        private readonly ConcurrentDictionary<Int16, PacketHandler<TPacket>> _handlers =
            new ConcurrentDictionary<Int16, PacketHandler<TPacket>>();

        protected PacketPropagatorBase()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                CacheHandlers(asm);
            Log.Info("{0} Packet Handler classes cached.", _handlers.Count);
        }

        public void Handle(IClient client, short typeId, byte[] payload, short length)
        {
            var handler = GetHandler(typeId);

#if DEBUG
            Log.Warn(
                "{0}  \n{1}/{2}:{3}\t{4}",
                client.ClientAddress,
                typeId.ToString("X2", CultureInfo.InvariantCulture),
                typeId,
                ((PacketTypes)typeId),
                length
                );

            //TODO: move and 'beautify' packet logging in the actual logger.
            for (int index = 0, l = 0; index < length; index++, l++)
                Console.Write("{0:X2}{1}", payload[index], (l % 16 == 0 && index != 0 ? "\n" : " "));
            Console.WriteLine();
#endif

            if (handler == null) return;

            var permission = handler.Permission;
            if (!client.HasPermission(permission))
            {
                Log.Warn("Client {0} sent type {1} which requires permission {2} - disconnected.",
                    client.ClientAddress,
                    typeId.ToString("X2", CultureInfo.InvariantCulture), permission.Name);
                client.Disconnect();
                return;
            }

            //leave as lambada, gives a complete stack trace.
            client.PostAsync(() => handler.Invoke(client, payload));
        }

        private void CacheHandlers(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                var attr = ReflectionExtensions.GetCustomAttribute<TAttribute>(type);
                if (attr == null) continue;

                var parent = typeof (TPacket);
                if (!type.IsAssignableTo(parent))
                {
                    Log.Error("{0} classes must inherit from {1}", attr, parent);
                    continue;
                }

                var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                    new[] {typeof (Byte[])}, null);
                if (ctor == null)
                {
                    Log.Error("{0} needs a public instance constructor with one type argument as a byte array.",
                        type.Name);
                    continue;
                }

                var typeId = attr.PacketTypeId;
                var handler = new PacketHandler<TPacket>(ctor, typeId, attr.Permission ?? typeof (ConnectedPermission));

                AddHandler(((IConvertible) typeId).ToInt16(null), handler);
            }
        }

        private void AddHandler(Int16 typeId, PacketHandler<TPacket> handler)
        {
            _handlers.Add(key: typeId, value: handler);
        }

        private PacketHandler<TPacket> GetHandler(Int16 typeId)
        {
            return _handlers.TryGet(typeId);
        }
    }
}