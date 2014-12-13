using System;
using System.Diagnostics.Contracts;
using System.Text;
using Throne.Shared.Math;

namespace Throne.Shared.Cryptography
{
    public class DiffieHellman
    {
        //TODO: Move prime and generator to config XML
        private static BigInteger requestModInteger;
        private readonly BigInteger generator;
        private readonly BigInteger primeRoot;
        public BigInteger Secret;
        private BigInteger publicRequestKey;


        public DiffieHellman(BigInteger p, BigInteger g)
        {
            primeRoot = p;
            generator = g;
        }

        public Byte[] GenerateRequest()
        {
            Contract.Assert(publicRequestKey == null);


            BigInteger request = generator;
            requestModInteger = BigInteger.ProbablePrime(256, new FastRandom());
            request = request.ModPow(requestModInteger, primeRoot);
            publicRequestKey = request;

            return Encoding.ASCII.GetBytes(publicRequestKey.ToString(16));
        }

        public Byte[] HandleResponse(BigInteger publicKey)
        {
            Secret = publicKey.ModPow(requestModInteger, primeRoot);
            return Secret.ToByteArrayUnsigned();
        }
    }
}