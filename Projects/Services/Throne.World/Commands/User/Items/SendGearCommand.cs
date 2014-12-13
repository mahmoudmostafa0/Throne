using Throne.Shared.Commands;
using Throne.World.Network.Messages;

namespace Throne.World.Commands.User.Items
{
    [Command("SendGear")]
    public sealed class SendGearCommand : UserCommand
    {
        public override string Description
        {
            get { return "Sends the gear of the user to the user."; }
        }

        public override void ExecuteUserCommand()
        {
            var pkt = new ItemAction().SendGear(Target);
            var ba = (byte[]) pkt;
            Target.User.Send(pkt);
        }
    }
}
