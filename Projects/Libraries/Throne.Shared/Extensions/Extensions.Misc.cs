using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;

namespace Throne.Framework
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Checks if an IntPtr object is null (0).
        /// </summary>
        /// <param name="ptr">The IntPtr object.</param>
        public static bool Null(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }

        /// <summary>
        ///     Checks if an UIntPtr object is null (0).
        /// </summary>
        /// <param name="ptr">The IntPtr object.</param>
        public static bool Null(this UIntPtr ptr)
        {
            return ptr == UIntPtr.Zero;
        }

        public static string ToHexString(this IntPtr pointer)
        {
            string stringRep = "0x" + pointer.ToString("X");
            return stringRep;
        }

        [StringFormatMethod("str")]
        public static string Interpolate(this string str, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, str, args);
        }

        public static bool IsBetween<T>(this T comparable, T lower, T upper)
            where T : IComparable<T>
        {
            return comparable.CompareTo(lower) >= 0 && comparable.CompareTo(upper) < 0;
        }

        /// <summary>
        ///     Performs an action with one parameter on a single object.
        /// </summary>
        /// <typeparam name="T">Type same as object to use on the action.</typeparam>
        /// <param name="obj"></param>
        /// <param name="act"></param>
        /// <returns>Object after the action.</returns>
        public static T With<T>(this T obj, Action<T> act)
            where T : class
        {
            act(obj);
            return obj;
        }


        public static string Format(this TimeSpan time)
        {
            return
                "{0}{1:00}h {2:00}m {3:00}s {4:00}ms".Interpolate(time.TotalDays > 0 ? (int) time.TotalDays + "d " : "",
                    time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }

        public static string FormatMillis(this DateTime time)
        {
            return "{0:00}h {1:00}m {2:00}s {3:00}ms".Interpolate(time.Hour, time.Minute,
                time.Second, time.Millisecond);
        }


        /// <summary>
        ///     Raises event with thread and null-ref safety.
        /// </summary>
        public static void Raise<T>(this Action<T> handler, T args)
        {
            if (handler != null)
                handler(args);
        }

        /// <summary>
        ///     Raises event with thread and null-ref safety.
        /// </summary>
        public static void Raise<T1, T2>(this Action<T1, T2> handler, T1 args1, T2 args2)
        {
            if (handler != null)
                handler(args1, args2);
        }

        /// <summary>
        ///     Raises event with thread and null-ref safety.
        /// </summary>
        public static void Raise<T1, T2, T3>(this Action<T1, T2, T3> handler, T1 args1, T2 args2, T3 args3)
        {
            if (handler != null)
                handler(args1, args2, args3);
        }

        public static String ToTQHex(this Color color)
        {
            return "0x{0}{1}{2}".Interpolate(color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2"));
        }
    }
}