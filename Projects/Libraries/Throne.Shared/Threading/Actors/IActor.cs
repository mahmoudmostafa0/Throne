using System;
using System.Threading.Tasks;
using Throne.Framework.Runtime;
using Throne.Framework.Security;

namespace Throne.Framework.Threading.Actors
{
    public interface IActor : IDisposableResource, IPermissible
    {
        void PostAsync(Action msg);

        Task PostWait(Action msg);
    }

    public interface IActor<out TThis> : IActor
        where TThis : IActor<TThis>
    {
        void PostAsync(Action<TThis> msg);

        Task PostWait(Action<TThis> msg);
    }
}