using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Throne.Shared.Reflection;

namespace Throne.Shared
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Casts one thing into another.
        /// </summary>
        /// <remarks>
        ///     This is a hack. It should only be used in rare cases.
        /// </remarks>
        public static T CastType<T>(this object obj)
        {

            var value = (T) Cast(obj, typeof (T));
            return value;
        }

        /// <summary>
        ///     Casts one thing into another.
        /// </summary>
        /// <remarks>
        ///     This is a hack. It should only be used in rare cases.
        /// </remarks>
        public static object Cast(this object obj, Type newType)
        {
            if (newType.IsEnum)
            {
                var str = obj as string;
                return str != null ? Enum.Parse(newType, str) : Enum.ToObject(newType, obj);
            }

            Type type = obj.GetType();
            if (type.IsInteger() && newType == typeof (bool)) // A hack for boolean values.
                return !obj.Equals(0.Cast(type));

            // Since we require that value is not null, the returned value won't be either.
            object value = Convert.ChangeType(obj, newType, CultureInfo.InvariantCulture);
            return value;
        }

        /// <summary>
        ///     Gets words and quoted word groups.
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks>
        ///     When a command is received the message is processed into arguments.
        ///     ie: > commandname arg1 arg2 "this would be arg3, but without the quotes" arg4
        /// </remarks>
        public static IEnumerable<String> ParseCommand(this string msg)
        {
            var matches = Regex.Matches(msg, @"(""[a-z0-9_\-\.,\+': ]+""|[a-z0-9_\-\.,\+':]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            for (var i = 0; i < matches.Count; i++)
                yield return matches[i].Groups[1].Value.Trim('"', ' ');
        }

        [CLSCompliant(false)]
        public static IConvertible AsConvertible<T>(this T value)
            where T : IConvertible
        {
            return value;
        }
    }
}