using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Throne.Framework;
using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.Login.Records;

namespace Throne.Login.Accounts
{
    public sealed class AccountManager : SingletonActor<AccountManager>
    {
        private static readonly LogProxy _log = new LogProxy("AccountManager");

        private readonly List<Account> _accounts = new List<Account>();

        private AccountManager()
        {
            _log.Info("Loading accounts...");

            IEnumerable<AccountRecord> accounts = AuthServer.Instance.AccountDbContext.FindAll<AccountRecord>();
            foreach (Account acc in accounts.Select(account => new Account(account)))
            {
                acc.Online = false;
                AddAccount(acc);
            }

            _log.Info("Loaded {0} accounts.", _accounts.Count);
        }

        public void AddAccount(Account acc)
        {
            _accounts.Add(acc);
        }

        public void RemoveAccount(Account acc)
        {
            _accounts.Remove(acc);
        }

        public Account CreateAccount(string userName, string password, string macaddress = "")
        {
            var rec = new AccountRecord(userName, password, macaddress);

            rec.Create();

            var acc = new Account(rec);
            AddAccount(acc);
            _log.Info("Account created.");
            return acc;
        }

        public void DeleteAccount(Account acc)
        {
            acc.Delete();
            RemoveAccount(acc);
        }

        /// <summary>
        /// Use to find multiple accounts matching a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<Account> FindAccounts(Func<Account, bool> predicate)
        {
            return _accounts.Where(predicate).Force();
        }

        public Boolean FindAccount(Func<Account, bool> predicate, out Account value)
        {
            value = _accounts.SingleOrDefault(predicate);
            return value;
        }

        public void UpdateAccount_Online(Int32 Guid, Boolean value)
        {
            var account = _accounts.SingleOrDefault(x => x.Guid == Guid);
            if (account != null)
                account.Online = value;
        }
    }
}