using Throne.Framework.Commands;

namespace Throne.World.Commands.User.Items
{
    [Command("CreateItem", "CI")]
    public sealed class CreateItem : UserCommand
    {
        public override string Description
        {
            get { return "Creates an item. (TypeID, Plus)"; }
        }

        public override void ExecuteUserCommand()
        {
            var item = ItemManager.Instance.CreateItem(Target, Arguments.NextInt32());
        }
    }
}
