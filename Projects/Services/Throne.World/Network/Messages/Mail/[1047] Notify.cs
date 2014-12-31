using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages.Mail
{
    public sealed class Notify : WorldPacket
    {
        public enum Types : ushort
        {
            DeletionFailed = 1,
            UnreadMail = 4
        }

        public Notify(Types type) : base(PacketTypes.MailNotify, 6 + 8)
        {
            WriteUShort((ushort) type);
        }
    }
}