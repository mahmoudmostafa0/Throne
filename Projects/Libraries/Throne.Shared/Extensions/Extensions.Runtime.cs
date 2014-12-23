using System;
using Throne.Framework.Runtime;

namespace Throne.Framework
{
    partial class Extensions
    {
        public static void ThrowIfDisposed(this IDisposableResource resource)
        {
            if (resource.IsDisposed)
                throw new ObjectDisposedException(resource.ToString(), "An attempt was made to use a disposed object.");
        }


    }
}
