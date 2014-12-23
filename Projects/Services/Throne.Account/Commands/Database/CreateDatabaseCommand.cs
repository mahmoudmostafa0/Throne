using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Throne.Framework.Commands;
using Throne.Framework.Security.Permissions;

namespace Throne.Login.Commands.Database
{
    [Command("CreateDb", "RecreateDb", "DbCreate", "DbRecreate")]
    public sealed class CreateDatabaseCommand : Command
    {
        public override string Description
        {
            get { return "Creates (or recreates) the database schema."; }
        }

        public override IEnumerable<Type> RequestedPermissions
        {
            get { yield return typeof (ConsolePermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            Console.WriteLine("Executing this command will permanently overwrite the entire database. Continue? (Y/N)");

            char answer = Console.ReadLine().ToUpper(CultureInfo.InvariantCulture).ToCharArray().FirstOrDefault();
            if (answer == 'Y')
                AuthServer.Instance.AccountDbContext.PostAsync(x => x.Schema.Create());
        }
    }
}