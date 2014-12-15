using Throne.Shared;
using Throne.Shared.Commands;
using Throne.World.Network;

namespace Throne.World.Commands.World
{
    [Command("MapInfo")]
    public sealed class MapInfoCommand : Command
    {
        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            var client = sender as WorldClient;
            if (!client) return;

            foreach (var neededPortal in client.Character.Location.Map.UnimplementedPortals)
                sender.Respond("ID:{0} {1}".Interpolate(neededPortal.Key, neededPortal.Value));
        }
    }
}
