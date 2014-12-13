using System;

namespace Throne.Shared.Math
{
    public class MathUtils
    {
        public static int BitFold32(int lower16, int higher16)
        {
            return (lower16) | (higher16 << 16);
        }

        public static void BitUnfold32(int bits32, out int lower16, out int upper16)
        {
            lower16 = bits32 & UInt16.MaxValue;
            upper16 = bits32 >> 16;
        }
    }
}