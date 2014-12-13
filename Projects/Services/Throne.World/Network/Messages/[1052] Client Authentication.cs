using Throne.Shared.Network.Connectivity;
using Throne.Shared.Network.Transmission.Stream;
using Throne.Shared.Security.Permissions;
using Throne.Shared.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Properties;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.AuthenticateClient)]
    public class ClientAuthentication : WorldPacket
    {
        private int password, session;

        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public ClientAuthentication(byte[] array) : base(array)
        {
        }

        public override bool Read(IClient client)
        {
            password = ReadInt();
            session = ReadInt();
            return true;
        }

        public override void Handle(IClient client)
        {
            WorldServer.Instance.AccountService.Call(
                accService =>
                {
                    if (!accService.Authorize(session, password))
                        return;

                    client.UserData = accService.GetAccount(session);
                    client.AddPermission(new AuthenticatedPermission());
                });

            if (client.HasPermission(typeof (AuthenticatedPermission)))
            {
                var @char = CharacterManager.Instance.FindCharacterRecord(client);
                if (@char == null)
                    using (new Stream()
                           + Constants.LoginMessages.NewRole
                           + Constants.LoginMessages.ServerInfo
                           > client) return;

                ((WorldClient) client).SetCharacter(CharacterManager.Instance.InitiaizeCharacter(client, @char));
            }
            else
                client.DisconnectWithMessage(Constants.LoginMessages.BadAuthentication);
        }
    }
}