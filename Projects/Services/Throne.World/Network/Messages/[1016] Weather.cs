using System;
using System.Drawing;
using Throne.Shared.Network.Transmission;

namespace Throne.World.Network.Messages
{
    public sealed class WeatherInformation : WorldPacket
    {
        public enum WeatherType : uint
        {
            None = 0,
            Fine,
            Rainy,
            Snowy,
            Sands,
            Leaf,
            Bamboo,
            Flower,
            Flying,
            Dandelion,
            Worm,
            Cloudy,
            All
        }

        public WeatherInformation(WeatherType type, Int32 direction, Int32 intensity, Color color)
            : base(PacketTypes.WeatherInformation, 28)
        {
            WriteUInt((UInt32)type);
            WriteInt(intensity);
            WriteInt(direction);
            WriteInt(color.ToArgb());
        }
    }
}
