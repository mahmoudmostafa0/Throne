using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Throne.Shared.Commands;
using Throne.Shared.Security.Permissions;
using Throne.World.Properties.Settings;

namespace Throne.World.Commands.Database
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
            get { yield return typeof(ConsolePermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            Console.WriteLine(StrRes.CMD_OverwriteDb);

            var answer = Console.ReadLine().ToUpper(CultureInfo.InvariantCulture).ToCharArray().FirstOrDefault();
           
            if (answer != 'Y') return;

            WorldServer.Instance.WorldDbContext.Schema.Create();
            WorldManager.Instance.PostAsync(mm => mm.InitDb());
        }
    }
}