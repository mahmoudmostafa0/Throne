using System;

namespace Throne.Shared.Runtime
{
    public interface IDisposableResource : IDisposable
    {
        Boolean IsDisposed { get; }
    }
}