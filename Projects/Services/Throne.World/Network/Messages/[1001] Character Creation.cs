using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Properties;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.Register)]
    public class CharacterCreation : WorldPacket
    {
        private Action _action;
        private Role.Profession _job;
        private String _macAddress;
        private Int16 _model;
        private String _name;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public CharacterCreation(Byte[] array)
            : base(array)
        {
        }

        public override bool Read(IClient client)
        {
            _action = (Action)ReadInt();
            _name = SeekForward(16).ReadString(16);
            _model = SeekForward(32).ReadShort();
            var professionSelection = ReadShort();
            SeekForward(4); //entity id?
            _macAddress = ReadString(16);

            switch (professionSelection)
            {
                case 0:
                case 1:
                    _job = Role.Profession.InternTaoist;
                    break;
                case 2:
                case 3:
                    _job = Role.Profession.InternTrojan;
                    break;
                case 4:
                case 5:
                    _job = Role.Profession.InternArcher;
                    break;
                case 6:
                case 7:
                    _job = Role.Profession.InternWarrior;
                    break;
                case 8:
                case 9:
                    _job = Role.Profession.InternNinja;
                    break;
                case 10:
                case 11:
                    _job = Role.Profession.InternMonkSaint;
                    break;
                case 12:
                case 13:
                    _job = Role.Profession.InternPirate;
                    break;
                case 14:
                case 15:
                    _job = Role.Profession.InternDragonWarrior;
                    break;
            }
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
                            if (mgr.CreateCharacter((WorldClient)client, _name, _job, _macAddress, _model))
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