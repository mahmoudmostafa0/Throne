using System;
using System.Collections.Generic;
using Throne.Framework.Commands;
using Throne.Framework.Security.Permissions;

namespace Throne.World.Commands.Database
{
    [Command("ExportSchema", "ExportDbSchema", "SchemaExport", "DbSchemaExport")]
    public sealed class ExportSchemaCommand : Command
    {
        public override string Description
        {
            get { return "Exports the database schema to a file."; }
        }

        public override IEnumerable<Type> RequestedPermissions
        {
            get { yield return typeof(ConsolePermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            string fileName = args.NextString();
            if (string.IsNullOrEmpty(fileName))
            {
                sender.Respond("No file name given.");
                return;
            }

            WorldServer.Instance.WorldDbContext.PostAsync(x => x.Schema.Export(fileName));
        }
    }
}