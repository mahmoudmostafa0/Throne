using System;
using System.ServiceModel;
using Throne.Login.Accounts;
using Throne.Framework.Services.Account;

namespace Throne.Login.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public sealed class AccountService : IAccountService
    {
        public AccountData? GetAccount(int session)
        {
            var acc = AccountManager.Instance.FindAccount(x => x.Guid == session);
            return acc != null ? acc.Serialize() : (AccountData?) null;
        }

        public Boolean Authorize(int session, int password)
        {
            var acc = AccountManager.Instance.FindAccount(x => x.Guid == session);
            if (acc == null) return false;
            if ((DateTime.Now - acc.LastLogin).Value.TotalSeconds > 2) return false;
            return acc.Password.GetHashCode() == password;
        }
    }
}