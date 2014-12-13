using System.Diagnostics.Contracts;

namespace Throne.Shared.Network.Security
{
    [ContractClass(typeof (PacketCipherContracts))]
    public interface INetworkCipher
    {
        byte[] Decrypt(byte[] buffer, int length);
        void Decrypt(byte[] packet, byte[] buffer, int length, int position);
        byte[] Encrypt(byte[] packet, int length);
        void GenerateKeys(int account, int authentication);
        void SetKey(byte[] key);
    }

    [ContractClassFor(typeof (INetworkCipher))]
    public abstract class PacketCipherContracts : INetworkCipher
    {
        public byte[] Encrypt(byte[] buffer, int count)
        {
            Contract.Requires(buffer != null);
            Contract.Requires(count >= 0);
            Contract.Requires(count <= buffer.Length);

            return null;
        }

        public byte[] Decrypt(byte[] buffer, int count)
        {
            Contract.Requires(buffer != null);
            Contract.Requires(count >= 0);
            Contract.Requires(count <= buffer.Length);

            return null;
        }

        public abstract void Decrypt(byte[] packet, byte[] buffer, int count, int position);

        public abstract void GenerateKeys(int account, int authentication);

        public void SetKey(byte[] key)
        {
            Contract.Requires(key != null);
        }
    }
}