using Throne.Shared.Commands;
using Throne.World.Network.Messages;
using Throne.World.Structures.Travel;

namespace Throne.World.Commands.User.Travel
{
    [Command("Teleport", "Warp", "Move", "Loc")]
    public sealed class TeleportCommand : UserCommand
    {
        public override string Description
        {
            get { return "Teleports the user to a new location. (MapID, X, Y)"; }
        }

        public override void ExecuteUserCommand()
        {
            var map = Arguments.NextUInt32();
            var x = Arguments.NextInt16();
            var y = Arguments.NextInt16();
            var dst = new Location(map, x, y);

            using (var pkt = new GeneralAction(ActionType.Teleport, Target).Teleport(dst))
                Target.User.Send(pkt);

            Target.EnterRegion(dst);
        }
    }
}
