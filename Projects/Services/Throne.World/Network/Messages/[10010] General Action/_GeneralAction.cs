using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.Framework.Security.Permissions;
using Throne.World.Network.Handling;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.Action, Permission = typeof (AuthenticatedPermission))]
    public sealed partial class GeneralAction : WorldPacket
    {
        private const Int32 SIZE = 50;

        private Character Character;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public GeneralAction(byte[] array)
            : base(array)
        {
        }

        /// <summary>
        ///     Outgoing Action message.
        /// </summary>
        public GeneralAction(ActionType type, WorldObject obj)
            : base(PacketTypes.Action, SIZE)
        {
            Type = type;
            ObjectId = obj.ID;
            SentTimestamp = Environment.TickCount;
            ProcessTimestamp = Environment.TickCount;

            //will be null if the object is not of the type.
            Character = obj as Character;
        }

        public Int32 SentTimestamp
        {
            get { return Seek(4).ReadInt(); }
            private set { Seek(4).WriteInt(value); }
        }

        public UInt32 ObjectId
        {
            get { return Seek(8).ReadUInt(); }
            set { Seek(8).WriteUInt(value); }
        }

        public Int64 Argument
        {
            get { return Seek(12).ReadLong(); }
            set { Seek(12).WriteLong(value); }
        }

        public Int32 ProcessTimestamp
        {
            get { return Seek(20).ReadInt(); }
            private set { Seek(20).WriteInt(value); }
        }

        public ActionType Type
        {
            get { return (ActionType) Seek(24).ReadShort(); }
            private set { Seek(24).WriteShort((Int16) value); }
        }

        public Orientation Direction
        {
            get { return (Orientation) Seek(26).ReadShort(); }
            set { Seek(26).WriteShort((Int16) value); }
        }

        public new Int32 this[Int32 ArgumentEx]
        {
            get { return Seek(ArgumentEx*4 + 28).ReadInt(); }
            set { Seek(ArgumentEx*4 + 28).WriteInt(value); }
        }

        public override bool Read(IClient client)
        {
            return true;
        }

        public override void Handle(IClient client)
        {
            Character = ((WorldClient) client).Character;

            client.Respond(Type.ToString());

            switch (Type)
            {
                    #region Logon Actions

                case ActionType.ConfirmLocation:
                    Character.User.Send(ConfirmLocation());
                    break;
                case ActionType.ConfirmAssets:
                    SendAssets(Character);
                    break;
                case ActionType.ConfirmAssociations:
                    SendFriends(Character);
                    break;
                case ActionType.ConfirmWeaponSkills:
                    SendProficiencies(Character);
                    break;
                case ActionType.ConfirmMagics:
                    SendSkills(Character);
                    break;
                case ActionType.ConfirmGuild:
                    SendGuild(Character);
                    break;
                case ActionType.CompleteLogin:
                    VerifyLogon(Character);
                    break;

                    #endregion

                case ActionType.UpdateCombatMode:
                    SetCombatMode();
                    break;
                case ActionType.Jump:
                    Jump();
                    break;
                case ActionType.QueryEntity:
                    QueryEntity();
                    break;
                case ActionType.Away:
                    Away();
                    break;
                case ActionType.ChangeMap:
                    UsePortal();
                    break;
                case ActionType.SetAppearance:
                    SetAppearance();
                    break;
                case ActionType.ChangeFace:
                    break;
            }
        }
    }
}