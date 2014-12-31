using System;
using System.Net;
using Throne.Framework.Persistence.Mapping;
using Throne.Framework.Services.Account;
using Throne.World.Database.Records.Implementations;

namespace Throne.World.Records
{
    public class AccountRecord : WorldDatabaseRecord
    {
        protected AccountRecord()
        {
        }

        public AccountRecord(AccountData loginData)
        {
            Guid = loginData.UserGuid;
            Username = loginData.Username;
            Password = loginData.Password;
            Email = loginData.Email;
            MacAddress = loginData.MacAddress;
            LastLogin = loginData.LastLogin;
            IP = loginData.LastIP;
            CreationTime = loginData.CreationTime;
        }

        public virtual int Guid { get; protected set; }

        public virtual string Username { get; protected set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual string MacAddress { get; set; }

        public virtual DateTime? LastLogin { get; set; }

        public virtual IPAddress IP { get; set; }

        public virtual DateTime? CreationTime { get; set; }

        public virtual Boolean Online { get; set; }

        public override void Update()
        {
            WorldServer.Instance.WorldDbContext.Update(this);
        }
    }

    public sealed class AccountMapping : MappableObject<AccountRecord>
    {
        public AccountMapping()
        {
            Id(r => r.Guid);

            Map(r => r.Username);
            Map(r => r.Password);
            Map(r => r.Email).Nullable();
            Map(r => r.MacAddress).Nullable();
            Map(r => r.LastLogin).Nullable();
            Map(r => r.IP).Nullable();
            Map(r => r.CreationTime).Nullable();
            Map(r => r.Online);
        }
    }
}