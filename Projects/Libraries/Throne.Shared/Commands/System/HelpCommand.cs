using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Throne.Shared.Commands.System
{
    [Command("Help", "Commands", "Command", "?")]
    public sealed class HelpCommand : Command
    {
        public override string Description
        {
            get { return "Lists all available commands."; }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            IDictionary<string, Command> commands = CommandManager.Instance.Commands;

            foreach (Command cmd in commands.Values.Distinct())
            {
                var permissions = cmd.RequestedPermissions;
                if (sender != null && permissions != null && !permissions.Any(sender.HasPermission))
                    continue;

                Command cmd1 = cmd;
                List<string> triggers = commands.Where(x => x.Value == cmd1).Select(x => x.Key).ToList();
                int count = triggers.Count;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < count; i++)
                {
                    sb.Append(triggers[i]);

                    if (i < count - 1)
                        sb.Append("|");
                }
                sb.Append(": {0}".Interpolate(cmd.Description));
                sender.Respond(sb.ToString());
            }
        }
    }
}