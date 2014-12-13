using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Throne.Login.Annotations;
using Throne.Login.Records;
using Throne.Shared;
using Throne.Shared.Collections;
using Throne.Shared.Logging;
using Throne.Shared.Threading;

namespace Throne.Login.Accounts
{
    [UsedImplicitly]
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
                Contract.Assume(acc != null);
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

        public IEnumerable<Account> FindAccounts(Func<Account, bool> predicate)
        {
            return _accounts.Where(predicate).Force();
        }

        public Account FindAccount(Func<Account, bool> predicate)
        {
            return _accounts.SingleOrDefault(predicate);
        }
    }
}