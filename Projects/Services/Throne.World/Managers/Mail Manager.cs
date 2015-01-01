using System;
using System.Collections.Generic;
using System.Linq;
using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.World.Database.Records;
using Throne.World.Database.Records.Implementations;
using Throne.World.Network.Messages;
using Throne.World.Network.Messages.Inbox;
using Throne.World.Security;
using Throne.World.Structures.Mail;
using Throne.World.Structures.Objects;

namespace Throne.World.Managers
{
    public sealed class MailManager : SingletonActor<MailManager>
    {
        private readonly LogProxy _log;
        private readonly SerialGenerator _serialGenerator;

        private MailManager()
        {
            _log = new LogProxy("MailManager");

            SerialGeneratorManager.Instance.GetGenerator(typeof (MailRecord).Name, WorldObject.ItemIdMin,
                WorldObject.ItemIdMax, ref _serialGenerator);
        }

        public void CheckUnread(Character forChr)
        {
            if (!forChr.Inbox.HasMail) return;
            if (!forChr.Inbox.UnreadMail) return;

            using (var pkt = new Notify(Notify.Types.UnreadMail))
                forChr.User.Send(pkt);
        }

        public void SendList(Character forChr, Int32 set)
        {
            Inbox inbox = forChr.Inbox;

            if (inbox.HasMail)
                PostAsync(delegate
                {
                    IEnumerable<Mail> enumerable = inbox.GetMails(set);
                    List<Mail> mail = enumerable.Skip(7*set).Take(7).ToList();
                    bool more = inbox.Count > 7*(set + 1);

                    using (var pkt = new List(mail, set, more))
                        forChr.User.Send(pkt);
                });
        }

        public void Open(Character forChr, UInt32 id)
        {
            PostAsync(delegate
            {
                Mail mail;
                if (forChr.Inbox.TryGetValue(id, out mail))
                    using (var mailPkt = new Content(mail.Content, id))
                        if (mail.Item)
                            using (var itemPkt = new ItemInformation(mail.Item, Item.Mode.Mail))
                                forChr.User.SendMany(mailPkt, itemPkt);
                        else forChr.User.Send(mailPkt);
                else
                    throw new ModerateViolation("Invalid mail ID.");
            });
        }

        public Boolean Delete(Character forChr, UInt32 id)
        {
            Mail mail;
            if (!forChr.Inbox.TryGetValue(id, out mail))
                throw new ModerateViolation("Invalid mail ID.");

            if (!mail.ContainsAttachment)
            {
                PostAsync(mail.Delete);
                forChr.Inbox.Remove(id);
                return true;
            }

            using (var pkt = new Notify(Notify.Types.DeletionFailed))
                forChr.User.Send(pkt);
            return false;
        }

        public Boolean RemoveItemAttachment(Character forChr, UInt32 id)
        {
            Mail mail;
            if (!forChr.Inbox.TryGetValue(id, out mail))
                throw new ModerateViolation("Invalid mail ID.");

            if (!mail.Item)
                throw new ModerateViolation("Invalid mail item.");

            PostWait(
                delegate
                {
                    forChr.AddItem(mail.Item);
                    mail.Item = null;
                }).Wait();

            return true;
        }

        public Boolean RemoveMoneyAttachment(Character forChr, UInt32 id)
        {
            Mail mail;
            if (!forChr.Inbox.TryGetValue(id, out mail))
                throw new ModerateViolation("Invalid mail ID.");

            PostWait(delegate
            {
                if (mail.Money > 0)
                    forChr.Money += mail.Money;
                mail.Money = 0;
            }).Wait();

            return true;
        }

        public Boolean RemoveEMoneyAttachment(Character forChr, UInt32 id)
        {
            Mail mail;
            if (!forChr.Inbox.TryGetValue(id, out mail))
                throw new ModerateViolation("Invalid mail ID.");

            PostWait(delegate
            {
                if (mail.EMoney > 0)
                    forChr.EMoney += mail.EMoney;
                mail.EMoney = 0;
            }).Wait();
                
            return true;
        }
    }
}