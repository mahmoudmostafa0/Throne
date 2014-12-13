using System;
using System.ServiceModel;

namespace Throne.Shared.Services
{
    public sealed class DuplexServiceClient<TService, TCallback> : DuplexClientBase<TService>
        where TService : class
        where TCallback : class, new()
    {

        public DuplexServiceClient(TCallback callback, Uri uri)
            : base(callback, new NetTcpBinding(SecurityMode.None, true), new EndpointAddress(uri))
        {
            CallbackChannel = callback;
        }

        public TService ServiceChannel
        {
            get { return Channel; }
        }

        public TCallback CallbackChannel { get; private set; }
    }
}