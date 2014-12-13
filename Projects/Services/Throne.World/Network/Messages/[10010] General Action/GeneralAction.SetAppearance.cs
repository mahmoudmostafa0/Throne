using Throne.Shared;
using Throne.World.Security;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        //TODO: Sanitize
        public void SetAppearance()
        {
            var appearance = (RoleAppearance)Argument;
            if (!appearance.IsValid())
                throw new SevereViolation("Player attempted to set an inappropriate appearance type.");

            Character.Appearance = appearance;
            ObjectId = Character.ID;
            Character.SendToLocal(this, true);
        }
    }
}
