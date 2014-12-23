using Throne.Framework;
using Throne.Framework.Commands;

namespace Throne.World.Commands.Scripting
{
    [Command("RefreshScripts", "ReloadScripts")]
    public sealed class RefreshScriptsCommand : Command
    {
        public override string Description
        {
            get { return "Refreshes all scripts in the scripting manager, recompiling stale scripts as needed."; }
        }

        public override bool RequiresSender
        {
            get { return true; }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            ScriptManager.Instance.Load();
            sender.Respond("All scripts have been refreshed.");
        }
    }
}
