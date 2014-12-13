using System;
using Throne.Shared.Runtime;
using Throne.Shared.Security;

namespace Throne.Shared.Threading.Actors
{
    public interface IActor : IDisposableResource, IPermissible
    {
        void Join();

        void PostAsync(Action msg);

        IWaitable PostWait(Action msg);
    }

    public interface IActor<out TThis> : IActor
        where TThis : IActor<TThis>
    {
        void PostAsync(Action<TThis> msg);

        IWaitable PostWait(Action<TThis> msg);
    }
}