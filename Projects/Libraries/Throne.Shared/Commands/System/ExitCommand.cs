using System;
using System.Collections.Generic;
using Throne.Shared.Security.Permissions;

namespace Throne.Shared.Commands.System
{
    [Command("Exit", "Close", "Bye", "Shutdown", "Die", "Kill")]
    public sealed class ExitCommand : Command
    {
        public override string Description
        {
            get { return "Tears down the process gracefully."; }
        }

        public override IEnumerable<Type> RequestedPermissions
        {
            get { yield return typeof (ConsolePermission); }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            CommandConsole.StopConsole = true;
        }
    }
}