using System;
using System.Collections.Generic;
using Throne.Login.Accounts;
using Throne.Framework;
using Throne.Framework.Commands;
using Throne.Framework.Security.Permissions;

namespace Throne.Login.Commands.Accounts
{
    [Command("CreateAccount", "AccountCreate", "AccCreate", "CreateAcc")]
    public sealed class CreateAccountCommand : Command
    {
        public override string Description
        {
            get { return "Creates and saves an account."; }
        }

        public override IEnumerable<Type> RequestedPermissions
        {
            get { yield return typeof (RootPermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            string name = args.NextString();
            string password = args.NextString();

            if (string.IsNullOrEmpty(name))
            {
                sender.Respond("No name given.");
                return;
            }

            if (name.Length < Constants.Accounts.MinNameLength || name.Length > Constants.Accounts.MaxNameLength)
            {
                sender.Respond(
                    "Name must be between {0} and {1} characters long.".Interpolate(Constants.Accounts.MinNameLength,
                        Constants.Accounts.MaxNameLength));
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                sender.Respond("No password given.");
                return;
            }

            if (password.Length < Constants.Accounts.MinPasswordLength ||
                password.Length > Constants.Accounts.MaxPasswordLength)
            {
                sender.Respond(
                    "Password must be between {0} and {1} characters long.".Interpolate(
                        Constants.Accounts.MinPasswordLength,
                        Constants.Accounts.MaxPasswordLength));
                return;
            }
            AccountManager.Instance.PostAsync(x => x.CreateAccount(name, password));
        }
    }
}