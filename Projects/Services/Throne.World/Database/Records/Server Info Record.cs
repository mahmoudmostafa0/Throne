using System;
using System.Linq;
using Throne.Shared.Persistence.Mapping;
using Throne.Shared.Threading;
using Throne.World.Database.Records.Implementations;

namespace Throne.World.Records
{
    public class ServerInfoRecord : WorldDatabaseRecord
    {
        public virtual String Name { get; set; }
        public virtual Int32 OnlineCount { get; set; }
    }

    public sealed class ServerInfoManager : SingletonActor<ServerInfoManager>
    {
        private ServerInfoManager() { }

        public ServerInfoRecord Get(String serverName)
        {
            var result =
                WorldServer.Instance.WorldDbContext.Find<ServerInfoRecord>(record => record.Name == serverName)
                    .SingleOrDefault();

            if (result != null) return result;

            result = new ServerInfoRecord {Name = serverName};
            result.Create();
            return result;
        }
    }

    public class ServerInfoMapping : MappableObject<ServerInfoRecord>
    {
        public ServerInfoMapping()
        {
            Id(r => r.Name).GeneratedBy.Assigned();

            Map(r => r.OnlineCount);
        }
    }
}
