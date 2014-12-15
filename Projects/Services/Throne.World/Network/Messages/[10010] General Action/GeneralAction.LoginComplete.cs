using System;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void VerifyLogon(Character chr)
        {
            using (ItemAction pkt = new ItemAction().SendGear(chr))
                chr.User.Send(pkt);

            chr.LoggedIn = true;

            chr.BeginWaitTask(chr.Save, new TimeSpan(0, 5, 0), new TimeSpan(0, 5, 0), CharacterTask.AutoSave);
        }
    }
}