using System;
using System.ServiceModel;

namespace Throne.Shared.Services
{
    public sealed class ServiceHost<TInterface, TService> : ServiceHost
        where TInterface : class
        where TService : class
    {
        public ServiceHost(TService instance, Uri uri)
            : base(instance)
        {
            AddServiceEndpoint(typeof(TInterface), new NetTcpBinding(SecurityMode.None, true), uri);
        }

        public TService Channel
        {
            get { return (TService)SingletonInstance; }
        }
    }
}