using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.Framework.Security.Permissions;
using Throne.World.Network.Messages.Mail;
using Throne.World.Structures.Travel;

namespace Throne.World.Network.Messages
{
    [Handling.WorldPacketHandler(PacketTypes.GroundMovement, Permission = typeof (AuthenticatedPermission))]
    public sealed class GroundMovement : WorldPacket
    {
        public enum Gait
        {
            Walk,
            Run,
            Shift,
            Jump,
            Trans,
            ChangeMap,
            JumpMagicAtttack,
            Collide,
            Synchronize,
            MountedSprint = 9
        }

        public UInt32 MapId;
        public UInt32 ObjectId;

        public Orientation Orientation;
        public Int32 Timestamp;
        public Gait Type;

        /// <summary>
        ///     Incoming message constructor
        /// </summary>
        /// <param name="payload">Incoming byte array</param>
        public GroundMovement(byte[] payload)
            : base(payload)
        {
        }

        public override bool Read(IClient client)
        {
            //The same unique ID appears at 8 and 20...
            Orientation = (Orientation) (ReadInt()%8);
            ObjectId = ReadUInt();
            Type = (Gait) ReadInt();
            Timestamp = ReadInt();
            MapId = ReadUInt();
            return true;
        }

        public override void Handle(IClient client)
        {
            var c = ((WorldClient) client).Character;
            c.GroundMovement(this);
        }
    }
}