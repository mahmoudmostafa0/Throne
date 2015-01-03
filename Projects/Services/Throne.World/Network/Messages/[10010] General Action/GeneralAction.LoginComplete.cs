using System;
using Throne.World.Managers;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void VerifyLogon(Character chr)
        {
            using (var pkt = new ItemAction().SendGear(chr))
                chr.User.Send(pkt);

            chr.LoggedIn = true;

            chr.BeginWaitTask(chr.Save, new TimeSpan(0, 5, 0), new TimeSpan(0, 5, 0), CharacterTask.AutoSave);
            
            MailManager.Instance.CheckUnread(chr);

            chr.LookAround();
        }
    }
}