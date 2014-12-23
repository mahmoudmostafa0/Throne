using System;
using Throne.Framework;
using Throne.Framework.Commands;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Structures.World;

namespace Throne.World.Commands.World
{
    [Command("MapAttribute")]
    public sealed class MapAttributeCommand : Command
    {
        public override string Description
        {
            get
            {
                return
                    "Set map attributes. (bool) Names: {0}".Interpolate(string.Join("/",
                        Enum.GetNames(typeof (MapAttribute))));
            }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            var character = ((WorldClient) sender).Character;
            var flag = args.NextEnum<MapAttribute>();
            var value = args.NextBoolean();
            var map = character.Location.Map;

            map[flag] = value;

            using (var pkt = new MapInfo(map))
                map.Send(pkt);
        }
    }
}