using System;
using Throne.World.Managers;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        private void VerifyLogon()
        {
            Character.User.Send(new ItemAction().SendGear(Character));

            Character.LoggedIn = true;
            Character.BeginWaitTask(Character.Save, new TimeSpan(0, 5, 0), new TimeSpan(0, 5, 0), CharacterTask.AutoSave);

            MailManager.Instance.CheckUnread(Character);

            Character.LookAround();
        }
    }
}