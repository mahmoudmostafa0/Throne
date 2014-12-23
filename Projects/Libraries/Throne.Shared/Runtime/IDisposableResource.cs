using System;

namespace Throne.Framework.Runtime
{
    public interface IDisposableResource : IDisposable
    {
        Boolean IsDisposed { get; }
    }
}