using System;
using System.ServiceModel;
using Throne.Framework.Exceptions;
using Throne.Framework.Threading.Actors;

namespace Throne.Framework.Services
{
    public sealed class IpcDevice<TService, TCallback> : Actor<IpcDevice<TService, TCallback>>
        where TService : class
        where TCallback : class, new()
    {
        private readonly Func<DuplexServiceClient<TService, TCallback>> _creator;
        private DuplexServiceClient<TService, TCallback> _client;

        public IpcDevice(Func<DuplexServiceClient<TService, TCallback>> clientCreator)
        {
            _creator = clientCreator;
            _client = clientCreator();
            _client.Open();
        }

        /// <summary>
        ///     Call remote remote process function.
        /// </summary>
        /// <param name="action"></param>
        public void Call(Action<TService> action)
        {
            try
            {
                action(_client.ServiceChannel);
            }
            catch (Exception ex)
            {
                if (ex is CommunicationException)
                {
                    Reconnect();
                    Call(action);
                    return;
                }
                ExceptionManager.RegisterException(ex);
            }
        }

        private void Connect()
        {
            _client = _creator();
            _client.Open();
        }

        private void Disconnect()
        {
            CommunicationState state = _client.State;
            if (state != CommunicationState.Closing && state != CommunicationState.Closed &&
                state != CommunicationState.Faulted)
                _client.Close();

            _client = null;
        }

        private void Reconnect()
        {
            CommunicationState state = _client.State;
            if (state == CommunicationState.Opening || state == CommunicationState.Opened)
                return;

            Disconnect();
            Connect();
        }

        protected override void Dispose(bool disposing)
        {
            if (_client != null)
                Disconnect();

            base.Dispose(disposing);
        }
    }
}