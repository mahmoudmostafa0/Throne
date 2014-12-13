using System;
using Throne.Shared.Network.Communication;
using Throne.Shared.Network.Transmission;
using Throne.Shared.Cryptography;

namespace Throne.World.Network.Exchange
{
    public class DiffieHellmanExchange : Packet
    {
        private const int
            PADDING_LENGTH = 11,
            JUNK_LENGTH = 10,
            ENCIV_LENGTH = 8,
            DECIV_LENGTH = 8,
            PRIMATIVE_ROOT_LENGTH = 128,
            PRIMARY_KEY_LENGTH = 128,
            GENERATOR_LENGTH = 2;

        public DiffieHellmanExchange(Byte[] encIv, Byte[] decIv)
            : base(333)
        {
            SeekForward(PADDING_LENGTH);
            WriteInt(ArrayLength - PADDING_LENGTH);
            WriteInt(JUNK_LENGTH);
            SeekForward(JUNK_LENGTH);
            WriteInt(DECIV_LENGTH);
            WriteArray(decIv);
            WriteInt(ENCIV_LENGTH);
            WriteArray(encIv);
            WriteInt(PRIMATIVE_ROOT_LENGTH);
            WriteArray(NetDragonDHKeyExchange.P);
            WriteInt(GENERATOR_LENGTH);
            WriteArray(NetDragonDHKeyExchange.G);
            WriteInt(PRIMARY_KEY_LENGTH);
            WriteArray(NetDragonDHKeyExchange.RequestKey);
            Seek(325).WriteString(TcpServer.OutgoingFooter);
        }
    }
}