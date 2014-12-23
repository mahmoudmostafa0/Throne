using System;
using System.Drawing;
using Throne.Framework;
using Throne.Framework.Commands;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Commands.World
{
    [Command("Weather")]
    public class WeatherCommand : Command
    {
        public override string Description
        {
            get
            {
                return
                    "Modifies this map's weather. (WeatherType, Intensity, Direction, Drawing.Color) | Names: {0}"
                        .Interpolate(string.Join("/", Enum.GetNames(typeof (WeatherInformation.WeatherType))));
            }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            using (
                var pkt = new WeatherInformation(args.NextEnum<WeatherInformation.WeatherType>(), args.NextInt32(75),
                    args.NextInt32(500), args.NextColor(KnownColor.LightSkyBlue)))
                ((WorldClient) sender).Send(pkt);
        }
    }
}