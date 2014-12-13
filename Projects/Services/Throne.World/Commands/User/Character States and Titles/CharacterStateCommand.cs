using Throne.Shared.Commands;
using Throne.World.Structures.Objects;

namespace Throne.World.Commands.User
{
    [Command("PStatus", "StatusEffect")]
    public sealed class AddStatusCommand : UserCommand
    {
        public override string Description
        {
            get { return "Adds or removes a public status. (Status, Boolean)"; }
        }

        public override void ExecuteUserCommand()
        {
            var type = Arguments.NextEnum<RoleState>();
            var value = Arguments.NextBoolean();

            Target.SetPublicState(type, value);
            Target.SendToLocal(Target);
        }
    }
}
