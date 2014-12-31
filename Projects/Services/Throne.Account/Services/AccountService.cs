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
        public AccountData GetAccount(int session)
        {
            Account result;
            AccountManager.Instance.FindAccount(x => x.Guid == session, out result);
            return result;
        }

        public Boolean Authorize(int session, int password)
        {
            var acc = GetAccount(session);
            if (!acc) return false;
            if ((DateTime.Now - acc.LastLogin).Value.TotalSeconds > 2) return false;
            return acc.Password.GetHashCode() == password;
        }

        public void SetOnline(Int32 guid, Boolean value)
        {
            AccountManager.Instance.UpdateAccount_Online(guid, value);
        }
    }
}