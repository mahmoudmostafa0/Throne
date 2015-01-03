using System;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.Framework.Security.Permissions;
using Throne.World.Network.Handling;
using Throne.World.Properties;
using Throne.World.Records;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.AuthenticateClient)]
    public class ClientAuthentication : WorldPacket
    {
        private int _password, _session;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public ClientAuthentication(byte[] array) : base(array)
        {
        }

        public override bool Read(IClient client)
        {
            _password = ReadInt();
            _session = ReadInt();
            return true;
        }

        public override void Handle(IClient client)
        {
            var c = (WorldClient) client;

            WorldServer.Instance.AccountService.Call(accService =>
            {
                if (!accService.Authorize(_session, _password))
                    return;

                c.AccountData = new AccountRecord(accService.GetAccount(_session));
                client.AddPermission(new AuthenticatedPermission());
            });

            if (client.HasPermission(typeof (AuthenticatedPermission)))
            {
                c.SendArrays(
                    Constants.LoginMessages.ServerInfo,
                    Constants.LoginMessages.AnswerOk,
                    new TimeSynchronize(DateTime.Now));

                CharacterRecord chr = CharacterManager.Instance.FindCharacterRecord(c);

                if (chr == null)
                {
                    client.Send(Constants.LoginMessages.NewRole);
                    return;
                }

                ((WorldClient) client).SetCharacter(CharacterManager.Instance.InitiaizeCharacter(c, chr));
            }
            else
                client.DisconnectWithMessage(Constants.LoginMessages.BadAuthentication);
        }
    }
}