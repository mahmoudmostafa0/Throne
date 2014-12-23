using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Handling;
using Throne.Framework.Network.Transmission;
using Throne.World.Database.Records;
using Throne.World.Properties;
using Throne.World.Records;

namespace Throne.World.Network.Messages
{
    [Handling.WorldPacketHandler(PacketTypes.Register)]
    public class CharacterCreation : WorldPacket
    {
        private Action _action;
        private Int16 _job;
        private String _macAddress;
        private Int16 _model;
        private String _name;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public CharacterCreation(Byte[] array) : base(array)
        {
        }

        public override bool Read(IClient client)
        {
            _action = (Action) ReadInt();
            _name = SeekForward(16).ReadString(16);
            _model = SeekForward(32).ReadShort();
            _job = ReadShort();
            _macAddress = SeekForward(4).ReadString(16);
            return true;
        }

        public override void Handle(IClient client)
        {
            if (_action == Action.Create)
                CharacterManager.Instance.PostAsync(
                    mgr =>
                    {
                        if (!mgr.NameValid(_name))
                            client.Send(Constants.CharacterManagementMessages.NameInvalid);
                        else
                        {
                            mgr.CreateCharacter(client, _name, (byte) _job, _macAddress, _model);
                            client.Send(Constants.CharacterManagementMessages.AnswerOk);
                        }
                    });
        }

        private enum Action
        {
            Create,
            Cancel
        }
    }
}