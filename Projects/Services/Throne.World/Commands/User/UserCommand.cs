using Throne.Framework;
using Throne.Framework.Commands;
using Throne.World.Network;
using Throne.World.Structures.Objects;

namespace Throne.World.Commands.User
{
    public abstract class UserCommand : Command
    {
        protected WorldClient Sender;
        protected Character Target;
        protected CommandArguments Arguments;

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            Sender = (WorldClient)sender;
            Arguments = args;

            if (args.TargetedCommand)
            {
                var targetName = args.NextString();

                if (targetName.Length > 16)
                    throw new CommandArgumentException("Name's too long yo!");
                if (targetName.Length < 3)
                    throw new CommandArgumentException("What kind of name is that? {0}? Little man with a littler name.".Interpolate(targetName));

                Target = CharacterManager.Instance.FindCharacter(targetName);
                if (!Target)
                    throw new CommandArgumentException("Who the hell is {0}?..".Interpolate(targetName));
            }
            else
                Target = Sender.Character;

            ExecuteUserCommand();
        }

        public abstract void ExecuteUserCommand();
    }
}
