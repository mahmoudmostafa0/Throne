using System;
using System.Security.Cryptography;
using System.Text;
using Throne.Shared.Collections;
using Throne.Shared.Math;

namespace Throne.Shared.Cryptography
{
    public sealed class NetDragonDHKeyExchange : DiffieHellman
    {
        public static readonly Byte[] G, P, RequestKey;

        private static readonly BigInteger GENERATOR, PRIME_ROOT;
        private static readonly MD5CryptoServiceProvider MD5;

        static NetDragonDHKeyExchange()
        {
            MD5 = new MD5CryptoServiceProvider();

            const String primeString = "E7A69EBDF105F2A6BBDEAD7E798F76A2"
                                       + "09AD73FB466431E2E7352ED262F8C558"
                                       + "F10BEFEA977DE9E21DCEE9B04D245F30"
                                       + "0ECCBBA03E72630556D011023F9E857F",
                generatorString = "05";

            PRIME_ROOT = new BigInteger(primeString);
            GENERATOR = new BigInteger(generatorString);

            G = Encoding.ASCII.GetBytes(generatorString);
            P = Encoding.ASCII.GetBytes(primeString);

            RequestKey = new DiffieHellman(PRIME_ROOT, GENERATOR).GenerateRequest();
        }

        public NetDragonDHKeyExchange()
            : base(PRIME_ROOT, GENERATOR)
        {
        }

        public static Byte[] ProcessDHSecret(Byte[] key)
        {
            string str1 = Hex(MD5.ComputeHash(key, 0, key.CountWhile(x => x != 0)));
            string str2 = Hex(MD5.ComputeHash(Encoding.ASCII.GetBytes(String.Concat(str1, str1))));
            string encryptedKey = String.Concat(str1, str2);
            return Encoding.ASCII.GetBytes(encryptedKey);
        }

        /// <summary>
        ///     Converts bytes to a ASCII hex string.
        ///     From COSV3
        /// </summary>
        private static string Hex(byte[] bytes)
        {
            var c = new char[bytes.Length*2];
            byte b;
            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte) (bytes[bx] >> 4));
                c[cx] = (char) (b > 9 ? b + 0x37 + 0x20 : b + 0x30);
                b = ((byte) (bytes[bx] & 0x0F));
                c[++cx] = (char) (b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }
            return new string(c);
        }
    }
}