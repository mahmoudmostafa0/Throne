using System.Diagnostics;
using System.Net;
using Throne.Framework.Network.Communication;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Properties;
using Throne.Framework.Threading;

namespace Throne.Framework.Network
{
    public abstract class NetworkApplication<T> : ActorApplication<T>
        where T : NetworkApplication<T>
    {
        protected NetworkApplication()
        {
            Server = CreateServer();

            TcpServer.ClientConnected += OnClientConnected;
            TcpServer.ClientDisconnected += OnClientDisconnected;
        }

        public TcpServer Server { get; private set; }
        public abstract IPEndPoint EndPoint { get; }

        public override void Start(string[] args)
        {
            base.Start(args);
            Log.Info(StrRes.NetworkAppStarted.Interpolate(Process.GetCurrentProcess().MaxWorkingSet));
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                TcpServer.ClientConnected -= OnClientConnected;
                TcpServer.ClientDisconnected -= OnClientDisconnected;
            }

            base.Dispose(disposing);
        }

        protected abstract void OnClientConnected(object sender, ConnectionEventArgs args);

        protected abstract void OnClientDisconnected(object sender, ConnectionEventArgs args);

        protected override void OnStop()
        {
            Server.Stop();
        }

        protected abstract TcpServer CreateServer();
    }
}