using System;
using Throne.Shared.Commands;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Commands.Npc
{
    [Command("AddNpc")]
    public sealed class AddNpcCommand : Command
    {
        private static Random rnd = new Random();
        public override string Description
        {
            get { return "Adds a new non-player character at this location."; }
        }

        public override void Execute(CommandArguments args, ICommandUser sender)
        {
            var client = sender as WorldClient;
            if (!client) return;
            var pos = client.Character.Location.Position.GetRandomLocal(3, 15, rnd);

            using (var pkt = new NpcInformation()
            {
                ID = (UInt32)rnd.Next(1, 700000),
                TypeFace = args.NextUInt16(3284),
                Type = args.NextEnum<NpcInformation.Types>(NpcInformation.Types.Talker),
                Unknown = args.NextInt32(0),
                X = pos.X,
                Y = pos.Y
            }) sender.Respond(pkt);
        }
    }
}