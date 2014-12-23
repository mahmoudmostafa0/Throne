using System;
using System.Diagnostics.Contracts;
using System.Net;
using Throne.Login.Annotations;
using Throne.Login.Network;
using Throne.Login.Network.Handling;
using Throne.Login.Network.Messages;
using Throne.Login.Properties;
using Throne.Login.Records.Implementations;
using Throne.Login.Services;
using Throne.Framework;
using Throne.Framework.Configuration;
using Throne.Framework.Network;
using Throne.Framework.Network.Communication;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Security.Permissions;
using Throne.Framework.Services;
using Throne.Framework.Cryptography;
using Throne.Framework.Services.Account;

namespace Throne.Login
{
    [UsedImplicitly]
    internal sealed class AuthServer : NetworkApplication<AuthServer>
    {
        #region Services

        public AccountDatabaseContext AccountDbContext { get; private set; }

        #endregion

        private ServiceHost<IAccountService, AccountService> _accountHost;
        private IPAddress _listenIp;

        private AuthServer()
        {
        }

        public override IPEndPoint EndPoint
        {
            get
            {
                Contract.Assume(GlobalDefaults.Default.ServerListenPort > IPEndPoint.MinPort);
                Contract.Assume(GlobalDefaults.Default.ServerListenPort < IPEndPoint.MaxPort);
                return new IPEndPoint(_listenIp, GlobalDefaults.Default.ServerListenPort);
            }
        }

        protected override void OnStart(string[] args)
        {
            if (!IPAddress.TryParse(GlobalDefaults.Default.ServerListenIP, out _listenIp))
                throw new ConfigurationValueException(StrRes.SMSG_ListenHostInvalid);

            if (!GlobalDefaults.Default.ServerListenPort.IsBetween(IPEndPoint.MinPort, IPEndPoint.MaxPort))
                throw new ConfigurationValueException(StrRes.SMSG_ListenPortInvalid);

            if (!GlobalDefaults.Default.ServerBacklog.IsBetween(10, 100))
                throw new ConfigurationValueException(StrRes.SMSG_BacklogRecommend);

            if (GlobalDefaults.Default.OutgoingPacketFooter != "" ||
                GlobalDefaults.Default.IncomingPacketFooter != "")
                throw new ConfigurationValueException(StrRes.SMSG_PacketFooterInvalid);

            Log.Info(StrRes.SMSG_ConfigPersist, GlobalDefaults.Default.DatabaseType);
            AccountDbContext = new AccountDatabaseContext(GlobalDefaults.Default.DatabaseType, GlobalDefaults.Default.DatabaseConnectionString);

            Log.Info(StrRes.SMSG_IPCAuthDeviceStartup, GlobalDefaults.Default.AccountServiceUri);
            _accountHost = new ServiceHost<IAccountService, AccountService>(new AccountService(), GlobalDefaults.Default.AccountServiceUri);
            _accountHost.Open();

            Log.Info(StrRes.SMSG_StartTCPListener, GlobalDefaults.Default.ServerName);
            Server.Start
                (
                    GlobalDefaults.Default.ServerName,
                    GlobalDefaults.Default.OutgoingPacketFooter,
                    GlobalDefaults.Default.IncomingPacketFooter,
                    EndPoint,
                    GlobalDefaults.Default.ServerBacklog,
                    GlobalDefaults.Default.FirewallWatchSeconds,
                    GlobalDefaults.Default.FirewallConnectionThreshold,
                    GlobalDefaults.Default.SocketNoNagel,
                    GlobalDefaults.Default.SocketKeepAlive,
                    GlobalDefaults.Default.SocketGracefulDisconnection,
                    GlobalDefaults.Default.SocketDontFragment,
                    GlobalDefaults.Default.SocketReuseEndpoint
                );
            Log.Info(StrRes.SMSG_TCPListenerStarted, GlobalDefaults.Default.ServerName, Server.LocalEndPoint);
            UpdateTitle();
        }



        protected override void OnStop()
        {
            Server.Stop();
            _accountHost.Close();
        }

        protected override TcpServer CreateServer()
        {
            return new TcpServer(new AuthenticationPacketPropagator());
        }

        protected override void OnClientConnected(object sender, ConnectionEventArgs args)
        {
            args.StreamCipher = new NetDragonAuthenticationCipher();

            var authClient = new AuthenticationClient(args);

            authClient.AddPermission(new ConnectedPermission());

            using (var packet = new AuthCipherSeed(authClient.SocketId))
                authClient.Send(packet);
        }        
        protected override void OnClientDisconnected(object sender, ConnectionEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void UpdateTitle()
        {
            Console.Title = ToString();
        }
    }
}