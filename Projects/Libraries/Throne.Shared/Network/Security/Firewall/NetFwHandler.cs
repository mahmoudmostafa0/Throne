using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using NetFwTypeLib;

namespace Throne.Framework.Network.Security.Firewall
{
    public class NetFwHandler
    {
        //TODO: Add more customizability and options.


        private readonly INetFwPolicy2 _firewall;
        private readonly ConcurrentDictionary<Int32, Int32> _list;
        private readonly Int32 _reset;

        private readonly Object _sync = new object();
        private readonly Int32 _tripConnections;

        private Int32 _nextreset;

        internal NetFwHandler(Int32 watchSeconds, Int32 connectionThreshold)
        {
            _firewall = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("hnetcfg.fwpolicy2"));
            _list = new ConcurrentDictionary<Int32, Int32>();
            _tripConnections = connectionThreshold;
            _reset = watchSeconds;
        }

        internal Boolean PassFwPolicy(IPAddress ip)
        {
            lock (_sync)
            {
                if (Environment.TickCount <= _nextreset)
                    return _list.AddOrUpdate(ip.GetHashCode(), i => 1, (i, i1) => i + 1) < _tripConnections;

                _list.Clear();
                _nextreset = Environment.TickCount + (_reset*1000);
                return true;
            }
        }

        public void AddRule(IPAddress ip, Int32 serverPort)
        {
            var rule = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID("hnetcfg.fwrule"));
            rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            rule.Enabled = true;
            rule.InterfaceTypes = "All";
            rule.Name = String.Format("[{0}] Activity restriction {1}:{2} - too many connections", DateTime.Now, ip,
                serverPort);
            rule.Protocol = 6; //tcp
            rule.RemoteAddresses = ip.ToString();
            rule.LocalPorts = serverPort.ToString(CultureInfo.InvariantCulture);

            _firewall.Rules.Add(rule);
        }
    }
}