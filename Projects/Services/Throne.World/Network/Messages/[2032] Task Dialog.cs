using System;
using System.Linq;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Sessions;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Task_Dialog;

namespace Throne.World.Network.Messages
{
    /// <summary>
    ///     Used to build interactive dialog windows in the client.
    /// </summary>
    [WorldPacketHandler(PacketTypes.TaskDialog)]
    public sealed class TaskDialog : WorldPacket
    {
        public enum Types
        {
            RemoveFurniture,
            Message,
            Option,
            Input,
            Picture,
            List,
            MessageBox,
            ShowDialog = 100,
            Feedback = 101,
            HideDialog = 102,

            //For "Updates" dialog.
            UpdatesDialog = 112,
            Header = 0,
            Body = 1,
            Footer = 2,
        }

        #region Receive

        private Character _character;

        public TaskDialog(Byte[] array)
            : base(array)
        {
        }

        public override bool Read(IClient client)
        {
            _character = ((WorldClient) client).Character;
            return true;
        }

        public override void Handle(IClient client)
        {
            NpcSession session = _character.NpcSession;
            var feedback = new DialogFeedback(Seek(14).ReadByte(),
                Seek(16).ReadStrings().FirstOrDefault() ?? String.Empty, Seek(4).ReadInt());

            if (session.Valid())
                if (session.State.Resume(feedback))
                    return;

            if (session.State != null)
                session.Clear();
        }

        #endregion

        #region Send

        public TaskDialog(Int32 stringLen)
            : base(PacketTypes.TaskDialog, 21 + stringLen + 8)
        {
            WriteInt(Environment.TickCount);
        }

        public TaskDialog Message(params String[] msg)
        {
            Seek(15).WriteByte((Byte) Types.Message);
            WriteStrings(msg);
            return this;
        }

        public TaskDialog Option(Byte op, String name)
        {
            Seek(14).WriteByte(op);
            WriteByte((Byte) Types.Option);
            WriteStrings(name);
            return this;
        }

        public TaskDialog Picture(UInt16 picId)
        {
            Seek(12).WriteUShort(picId);
            Seek(15).WriteByte((Byte) Types.Picture);
            return this;
        }

        public TaskDialog Input(Byte op, UInt16 szInput, String name)
        {
            Seek(12).WriteUShort(szInput);
            WriteByte(op);
            WriteByte((Byte) Types.Input);
            WriteStrings(name);
            return this;
        }

        public TaskDialog ShowDialog()
        {
            Seek(15).WriteByte((Byte) Types.ShowDialog);
            return this;
        }

        #endregion
    }
}