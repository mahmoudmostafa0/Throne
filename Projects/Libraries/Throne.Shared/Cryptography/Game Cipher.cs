using System;
using System.Text;
using Throne.Shared.Network.Security;

namespace Throne.Shared.Cryptography
{
    public unsafe class GameCipher : CAST5, INetworkCipher
    {
        public static Byte[] encryptionIv;
        public static Byte[] decryptionIv;

        static GameCipher()
        {
            encryptionIv = new byte[8];
            decryptionIv = new byte[8];
        }

        public GameCipher(String CAST5_STANDARD)
        {
            //TODO: Make the CAST5 standard key static.
            GenerateKey(Encoding.ASCII.GetBytes(CAST5_STANDARD));
        }

        public byte[] Decrypt(byte[] stream, int length)
        {
            //throneGameCipher.Decrypt(stream, stream.Length);
            fixed (byte* p = stream) Decrypt(p, length);
            return stream;
        }

        public void Decrypt(byte[] packet, byte[] buffer, int length, int position)
        {
            //throneGameCipher.Decrypt(packet, length, position);
            fixed (byte* p = packet) Decrypt(p, length);
        }

        public byte[] Encrypt(byte[] packet, int length)
        {
            fixed (byte* p = packet) Encrypt(p, length);
            return packet;
        }

        public void GenerateKeys(int account, int authentication)
        {
            throw new NotImplementedException();
        }

        public void SetKey(byte[] secret)
        {
            GenerateKey(secret);

            fixed (byte* serverIv = encryptionIv)
            fixed (byte* clientIv = decryptionIv)
                SetIVs(serverIv, clientIv);
        }
    }
}