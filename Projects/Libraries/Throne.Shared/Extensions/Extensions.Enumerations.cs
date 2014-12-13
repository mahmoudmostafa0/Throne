using System;

namespace Throne.Shared
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Checks if a given enum value has any of the given enum flags.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="toTest">The flags to test.</param>
        public static bool HasAnyFlag(this Enum value, Enum toTest)
        {
            ulong val = ((IConvertible)value).ToUInt64(null);
            ulong test = ((IConvertible)toTest).ToUInt64(null);

            return (val & test) != 0;
        }

        /// <summary>
        ///     Checks if an enum value is valid (that is, defined in the enumeration type).
        /// </summary>
        /// <param name="value">The value to check.</param>
        public static bool IsValid(this Enum value)
        {
            return Enum.IsDefined(value.GetType(), value);
        }

        /// <summary>
        ///     Ensures that a given enum value is valid (defined). Throws if not.
        /// </summary>
        /// <param name="value">The value to check.</param>
        public static Enum EnsureValid(this Enum value)
        {
            if (!value.IsValid())
                throw new ArgumentException("Enum value is not valid.");

            return value;
        }
    }
}
