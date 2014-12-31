using System;
using System.Net;
using Throne.Login.Records;
using Throne.Framework.Runtime;
using Throne.Framework.Services.Account;

namespace Throne.Login.Accounts
{
    public sealed class Account : IMemberwiseSerializable<AccountData>
    {
        public Account(AccountRecord record)
        {
            Record = record;
        }

        /// <summary>
        ///     Gets the underlying record of this Account. Should not be manipulated
        ///     directly.
        /// </summary>
        public AccountRecord Record { get; private set; }

        public int Guid
        {
            get { return Record.Guid; }
            set { throw new NotImplementedException(); }
        }

        public Boolean Online
        {
            get { return Record.Online; }
            set
            {
                Record.Online = value;
                Record.Update();
            }
        }

        public string Username
        {
            get
            {
                string name = Record.Username;
                return name;
            }
        }

        public string EmailAddress
        {
            get
            {
                string email = Record.Email;
                return email;
            }
            set
            {
                Record.Email = value;
                Record.Update();
            }
        }

        public string MacAddress
        {
            get { return Record.MacAddress; }
            set
            {
                Record.MacAddress = value;
                Record.Update();
            }
        }

        public String Password
        {
            get { return Record.Password; }
            set
            {
                Record.Password = value;
                Record.Update();
            }
        }

        public DateTime? LastLogin
        {
            get { return Record.LastLogin; }
            set
            {
                Record.LastLogin = value;
                Record.Update();
            }
        }

        public IPAddress LastIP
        {
            get
            {
                //TODO: No need for IP addresses being stored. We have mac addresses. Use IPs only for geolocation, set only by the auth server.
                return Record.IP;
            }
            set
            {
                Record.IP = value;
                Record.Update();
            }
        }

        /// <summary>
        ///     This method prepares the account info for IPC transfer which usually
        ///     is to the world server for authentication.
        /// </summary>
        public AccountData Serialize()
        {
            return new AccountData
            {
                UserGuid = Guid,
                Username = Username,
                Email = EmailAddress,
                LastIP = LastIP,
                LastLogin = LastLogin,
                MacAddress = MacAddress,
                Password = Password,
                Online = Online,
                CreationTime = Record.CreationTime
            };
        }

        public static implicit operator AccountData(Account acc)
        {
            return acc ? acc.Serialize() : null;
        }

        public static implicit operator Boolean(Account acc)
        {
            return acc != null;
        }

        /// <summary>
        ///     Deletes the Account from the backing storage. This object is considered invalid once
        ///     this method has been executed.
        /// </summary>
        public void Delete()
        {
            Record.Delete();
        }

        public void ChangePassword(string password)
        {
            Password = password;
        }
    }
}