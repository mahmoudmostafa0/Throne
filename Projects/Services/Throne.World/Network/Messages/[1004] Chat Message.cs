using System;
using System.Drawing;
using System.Runtime.Remoting;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Network.Transmission;
using Throne.World.Network.Handling;
using Throne.World.Properties.Settings;
using Throne.World.Security;
using Throne.World.Structures.Objects;

namespace Throne.World.Network.Messages
{
    [WorldPacketHandler(PacketTypes.ChatMessage)]
    public class ChatMessage : WorldPacket
    {
        public const String SYSTEM = "SYSTEM", ALLUSERS = "ALLUSERS";
        private const Int32 MINIMUM_LENGTH = 33, MINIMUM_STRINGS = 7;

        public Color Color;

        public WorldClient Client;

        public UInt32 Identity;

        public String
            Message,
            MessagePrefix = "",
            MessageSuffix = "",
            Recipient = "",
            Sender = "";

        public UInt32 RecipientMesh;
        public UInt32 SenderMesh;
        public MessageStyle Style;
        public MessageChannel Type;
        public String[] Strings = new String[7];
        private Boolean received;

        public ChatMessage(MessageChannel type, String message)
            : base(0)
        {
            TypeId = (short)PacketTypes.ChatMessage;

            Type = type;
            Style = MessageStyle.Normal;
            Color = Color.White;
            Sender = SYSTEM;
            Recipient = ALLUSERS;
            Message = message;
        }

        public ChatMessage(MessageChannel type, String message, Role to)
            : base(0)
        {
            TypeId = (short)PacketTypes.ChatMessage;

            Type = type;
            Style = MessageStyle.Normal;
            Color = Color.White;
            Sender = SYSTEM;
            Identity = to.ID;
            Recipient = to.Name;
            Message = message;
        }

        public ChatMessage(MessageChannel type, String message, MessageStyle style, Color color)
            : base(0)
        {
            TypeId = (short)PacketTypes.ChatMessage;

            Type = type;
            Style = style;
            Color = color;
            Sender = SYSTEM;
            Recipient = ALLUSERS;
            Message = message;
        }



        /// <summary>
        ///     Incoming constructor.
        /// </summary>
        /// <param name="array">Incoming byte array.</param>
        public ChatMessage(byte[] array)
            : base(array)
        {
            received = true;
        }

        public override bool Read(IClient client)
        {
            Client = (WorldClient)client;

            ReadInt();
            Color = Color.FromArgb(ReadInt());
            Type = (MessageChannel)ReadUShort();
            Style = (MessageStyle)ReadUShort();
            Identity = ReadUInt();
            RecipientMesh = ReadUInt();
            SenderMesh = ReadUInt();
            Strings = ReadStrings();

            if (Strings.Length < MINIMUM_STRINGS)
                throw new SevereViolation(StrRes.SMSG_ChatMessageBadStrings);

            Sender = Strings[0];
            Recipient = Strings[1];
            MessageSuffix = Strings[2];
            Message = Strings[3];

            if (Sender != Client.Character.Name)
                throw new SevereViolation(StrRes.SMSG_ChatMessageInvalidSender, Sender, Client.Character.Name);

            if (!string.IsNullOrWhiteSpace(Message)) return true;
#if DEBUG
            Log.Info(StrRes.SMSG_ChatMessageEmpty);
#endif
            return false;
        }

        public Int32 Length
        {
            get
            {
                return
                    MINIMUM_LENGTH +
                    Recipient.Length +
                    Sender.Length +
                    MessageSuffix.Length +
                    Message.Length +
                    MessagePrefix.Length;
            }
        }

        public override void Handle(IClient client)
        {
            Seek(4);
            ChatManager.Instance.PostAsync(cm => cm.ProcessChatMessage(this));
        }

        protected override byte[] Build()
        {
            Resize(Length + 8);
            Seek(4);
            WriteInt(Environment.TickCount);
            WriteInt(Color.ToArgb());
            WriteUShort((ushort)Type);
            WriteUShort((ushort)Style);
            WriteUInt(Identity);
            WriteUInt(RecipientMesh);
            WriteUInt(SenderMesh);
            WriteStrings(Sender, Recipient, MessageSuffix, Message, String.Empty, String.Empty, MessagePrefix);
            return base.Build();
        }
    }

    public enum MessageChannel : ushort
    {
        Talk = 2000,
        Whisper = 2001,
        Action = 2002,
        Team = 2003,
        Guild = 2004,
        TopLeft = 2005,
        Clan = 2006,
        System = 2007,
        Yell = 2008,
        Friends = 2009,
        Center = 2011,
        Ghost = 2013,
        Service = 2014,
        Tip = 2015,
        Horn = 2016,
        ServerBroadcast = 2017,
        World = 2021,
        ArenaQualifier = 2022,
        Study = 2024,
        Ally = 2025,
        JiangHu = 2026,

        /// <summary>  Used for message popups in-game and at character creation. </summary>
        Popup = 2100,
        Login = 2101,
        Shop = 2102,
        Pet = 2103,
        Cryout = 2104,
        Website = 2105,
        StartRight = 2108,
        ContinueRight = 2109,
        LeaveMessage = 2110,
        GuildBulletin = 2111,
        SystemCenter = 2115,
        TradeBoard = 2201,
        FriendBoard = 2202,
        TeamBoard = 2203,
        GuildBoard = 2204,
        OtherBoard = 2205,
        CrossServerBroadcast = 2401,
        CrossServerChat = 2402,
        BroadcastMessage = 2500,
        MonsterTalk = 2600
    }

    [Flags]
    public enum MessageStyle : ushort
    {
        Normal = 0,
        Scroll = 1 << 0,
        Flash = 1 << 1,
        Blast = 1 << 2
    }

    public enum MessageColor
    {
        None = -1,
        White = 0xFFFFFF,
        Black = 0x000000,
        Yellow = 0x00FFFF,
        Pink = 0xFF00FF,
        Green = 0x00FF00,
        Blue = 0x0000FF,
        Red = 0xFF0000
    }
}