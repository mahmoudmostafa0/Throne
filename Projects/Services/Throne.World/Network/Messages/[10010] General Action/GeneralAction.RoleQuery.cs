using System;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void QueryEntity()
        {
            //TODO: Monsters and such?
            var requested = Character.GetVisibleCharacter((UInt32)Argument);
            if (requested)
                requested.ExchangeSpawns(Character);
        }
    }
}
