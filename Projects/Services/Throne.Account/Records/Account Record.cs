using System;
using System.Net;
using Throne.Login.Records.Implementations;
using Throne.Shared.Persistence.Mapping;
using Throne.Shared;

namespace Throne.Login.Records
{
    public class AccountRecord : AccountDatabaseRecord
    {
        /// <summary>
        ///     Constructs a new AccountRecord object.
        ///     This should be used only by the underlying database layer.
        /// </summary>
        protected AccountRecord()
        {
        }

        /// <summary>
        ///     Constructs a new AccountRecord object.
        ///     Should be inserted into the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="macaddress"></param>
        public AccountRecord(string username, string password, string macaddress)
        {
            Username = username;
            Password = password;
            MacAddress = macaddress;
            CreationTime = DateTime.Now;
        }

        public virtual int Guid { get; protected set; }

        public virtual string Username { get; protected set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual string Fingerprint { get; set; }

        public virtual string MacAddress { get; set; }

        public virtual DateTime? LastLogin { get; set; }

        public virtual IPAddress LastIP { get; set; }

        public virtual DateTime? CreationTime { get; set; }
    }

    public sealed class AccountMapping : MappableObject<AccountRecord>
    {
        public AccountMapping()
        {
            Id(r => r.Guid);
            Map(r => r.Username).Length(Constants.Accounts.MaxNameLength);
            Map(r => r.Password).Length(Constants.Accounts.MaxPasswordLength);
            Map(r => r.Email).Nullable();
            Map(r => r.Fingerprint).Length(Constants.Accounts.FingerprintLength).Nullable();
            Map(r => r.MacAddress).Length(Constants.Accounts.MacAddressLength).Nullable();
            Map(r => r.LastLogin).Nullable();
            Map(r => r.LastIP).Nullable();
            Map(r => r.CreationTime).Nullable();
        }
    }
}