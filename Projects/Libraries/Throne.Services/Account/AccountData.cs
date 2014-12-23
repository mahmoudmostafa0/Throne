using System;
using System.Net;
using System.Runtime.Serialization;

namespace Throne.Framework.Services.Account
{
    [DataContract]
    public struct AccountData
    {
        [DataMember] public string Email;
        [DataMember] public string Fingerprint;
        [DataMember] public IPAddress LastIP;
        [DataMember] public DateTime? LastLogin;
        [DataMember] public string MacAddress;
        [DataMember] public string Password;
        [DataMember] public Int32 UserGuid;
        [DataMember] public string Username;
    }
}