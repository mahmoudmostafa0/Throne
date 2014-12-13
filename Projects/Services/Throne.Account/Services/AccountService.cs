using System;
using System.ServiceModel;
using Throne.Login.Accounts;
using Throne.Shared.Services.Account;

namespace Throne.Login.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public sealed class AccountService : IAccountService
    {
        public AccountData? GetAccount(int session)
        {
            Account acc = null;
            AccountManager.Instance.PostWait(mgr => acc = mgr.FindAccount(x => x.Guid == session)).Wait();

            return acc != null ? acc.Serialize() : (AccountData?) null;
        }

        public Boolean Authorize(int session, int password)
        {
            Account acc = null;
            AccountManager.Instance.PostWait(mgr => acc = mgr.FindAccount(x => x.Guid == session)).Wait();

            if (acc == null) return false;
            if ((DateTime.Now - acc.LastLogin).Value.TotalSeconds > 2) return false;
            return acc.Password.GetHashCode() == password;
        }
    }
}