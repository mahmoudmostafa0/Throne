using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Throne.Framework.Commands;
using Throne.Framework.Security.Permissions;
using Throne.World.Properties.Settings;

namespace Throne.World.Commands.Database
{
    [Command("DropDb", "DbDrop")]
    public sealed class DropDatabaseCommand : Command
    {
        public override string Description
        {
            get { return "Drops the database schema and all contained data."; }
        }

        public override IEnumerable<Type> RequestedPermissions
        {
            get { yield return typeof (ConsolePermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            Console.WriteLine(StrRes.CMD_DropDb);

            char answer = Console.ReadLine().ToUpper(CultureInfo.InvariantCulture).ToCharArray().FirstOrDefault();
            if (answer == 'Y')
                WorldServer.Instance.WorldDbContext.PostAsync(x => x.Schema.Drop());
        }
    }
}