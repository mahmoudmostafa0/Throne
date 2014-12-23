using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Threading;
using Throne.Framework;
using Throne.Framework.Configuration;
using Throne.Framework.Cryptography;
using Throne.Framework.Network;
using Throne.Framework.Network.Communication;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Security.Permissions;
using Throne.Framework.Services;
using Throne.Framework.Services.Account;
using Throne.World.Database.Records.Implementations;
using Throne.World.Network;
using Throne.World.Network.Handling;
using Throne.World.Properties.Settings;
using Throne.World.Records;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World
{
    internal sealed class WorldServer : NetworkApplication<WorldServer>
    {
        private IPAddress _listenIp;

        private WorldServer()
        {
        }

        public EventManager Events { get; private set; }
        public ServerInfoRecord Info { get; private set; }

        public WorldDatabaseContext WorldDbContext { get; private set; }

        public IpcDevice<IAccountService, EmptyCallbackService> AccountService { get; private set; }

        public override IPEndPoint EndPoint
        {
            get
            {
                Contract.Assume(_listenIp != null);
                Contract.Assume(SystemSettings.Default.ServerListenPort > IPEndPoint.MinPort);
                Contract.Assume(SystemSettings.Default.ServerListenPort < IPEndPoint.MaxPort);
                return new IPEndPoint(_listenIp, SystemSettings.Default.ServerListenPort);
            }
        }

        protected override void OnClientConnected(object sender, ConnectionEventArgs args)
        {
            args.StreamCipher = new GameCipher(SystemSettings.Default.EncCAST5Standard);

            var worldClient = new WorldClient(args);
            worldClient.AddPermission(new ConnectedPermission());
            Info.OnlineCount++;
            Info.Update();
        }

        protected override void OnClientDisconnected(object sender, ConnectionEventArgs args)
        {
            Info.OnlineCount--;
            Info.Update();
        }


        protected override void OnStart(string[] args)
        {
            if (!IPAddress.TryParse(SystemSettings.Default.ServerListenIP, out _listenIp))
                throw new ConfigurationValueException(StrRes.SMSG_ListenHostInvalid);

            if (!SystemSettings.Default.ServerListenPort.IsBetween(IPEndPoint.MinPort, IPEndPoint.MaxPort))
                throw new ConfigurationValueException(StrRes.SMSG_ListenPortInvalid);

            if (!SystemSettings.Default.ServerBacklog.IsBetween(10, 100))
                throw new ConfigurationValueException(StrRes.SMSG_BacklogRecommend);

            if (SystemSettings.Default.OutgoingPacketFooter != "" &&
                !SystemSettings.Default.OutgoingPacketFooter.Length.Equals(8) |
                SystemSettings.Default.IncomingPacketFooter != "" &&
                !SystemSettings.Default.IncomingPacketFooter.Length.Equals(8))
                throw new ConfigurationValueException(StrRes.SMSG_PacketFooterInvalid);

            Log.Info(StrRes.SMSG_ConfigPersist, SystemSettings.Default.DatabaseType);
            WorldDbContext = new WorldDatabaseContext(SystemSettings.Default.DatabaseType,
                SystemSettings.Default.DatabaseConnectionString);

            Log.Info(StrRes.SMSG_IPCAuthDeviceConnect);
            AccountService = new IpcDevice<IAccountService, EmptyCallbackService>(() =>
                new DuplexServiceClient<IAccountService, EmptyCallbackService>(new EmptyCallbackService(),
                    SystemSettings.Default.AccountServiceUri));
            Log.Info(StrRes.SMSG_IPCAuthDeviceConnected);

            Info = ServerInfoManager.Instance.Get(SystemSettings.Default.ServerName);

            Events = new EventManager();

            ScriptManager.Instance.Load();


            Log.Info(StrRes.SMSG_StartTCPListener, SystemSettings.Default.ServerName, EndPoint.Address, EndPoint.Port);
            Server.Start
                (
                    SystemSettings.Default.ServerName,
                    SystemSettings.Default.OutgoingPacketFooter,
                    SystemSettings.Default.IncomingPacketFooter,
                    EndPoint,
                    SystemSettings.Default.ServerBacklog,
                    SystemSettings.Default.FirewallWatchSeconds,
                    SystemSettings.Default.FirewallConnectionThreshold,
                    SystemSettings.Default.SocketNoNagel,
                    SystemSettings.Default.SocketKeepAlive,
                    SystemSettings.Default.SocketGracefulDisconnection,
                    SystemSettings.Default.SocketDontFragment,
                    SystemSettings.Default.SocketReuseEndpoint
                );
            UpdateTitle();
        }

        protected override void OnStop()
        {
            AccountService.Dispose();
        }

        protected override TcpServer CreateServer()
        {
            return new TcpServer(new WorldPacketPropagator());
        }

        protected override void Pulse(TimeSpan diff)
        {
        }

        public void UpdateTitle()
        {
            Console.Title = ToString();
        }

        public override string ToString()
        {
            return "{0} - {1}".Interpolate(SystemSettings.Default.ServerName, base.ToString());
        }
    }
}