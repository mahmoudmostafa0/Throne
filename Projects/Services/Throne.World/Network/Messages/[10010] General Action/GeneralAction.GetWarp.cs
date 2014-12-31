using Throne.Framework;
using Throne.World.Properties.Settings;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Travel;
using Throne.World.Structures.World;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public void UsePortal()
        {
            //no client network info is read here so nothing can be exploited

            var map = Character.Location.Map;
            var pos = Character.Location.Position;
            var cellInf = Character.Location.Map.GetCell(pos);

            if (!cellInf[CellType.Portal])
                Character.Log.Error(StrRes.MSG_NoPortalHere.Interpolate(Character.Location));

            Character.ExitCurrentRegion();

            var dst = Location.None;
            using (var pkt = new GeneralAction(ActionType.Teleport, Character))
                if (!(map.Script is DummyMapScript) && map.Script.Warps.TryGetValue(cellInf.Argument, out dst))
                    Character.User.Send(pkt.Teleport(dst));
                else
                {
                    Character.User.Send(pkt.Teleport(dst = map.ReviveLocation));
                    Log.Error(StrRes.SMSG_WarpNotFound, cellInf.Argument, map.Id);
                }

            Character.EnterRegion(dst.Copy);
        }
    }
}