using System;
using Throne.Framework.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public sealed class TimeSynchronize : WorldPacket
    {
        public TimeSynchronize(DateTime time) : base(PacketTypes.TimeSynchronize, 48)
        {
            WriteInt(Environment.TickCount);
            WriteUInt(0); //time keeper type.. 0 = server time
            WriteInt(time.Year - 1900);
            WriteInt(time.Month - 1);
            WriteInt(time.DayOfYear);
            WriteInt(time.Day);
            WriteInt(time.Hour);
            WriteInt(time.Minute);
            WriteInt(time.Second);
        }
    }
}