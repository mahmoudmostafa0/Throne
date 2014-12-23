using System;
using System.Threading.Tasks;
using Throne.Framework;
using Throne.World.Network.Messages;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Travel;

public sealed class GuildDirector : NpcScript
{
    public override void Load()
    {
        SetFace(88);
        SetMesh(15);
        SetIdentity(10002);
        SetFacing(Orientation.Southwest);
        SetType(NpcInformation.Types.Talker);
        SetLocation(new Location(1002, new Position(269, 291)));
    }

    protected override async Task Talk()
    {
        Message("Hello {0}, this is Throne's first interactive dialog.".Interpolate(Character.Name), 
            Option("Option 0 (Close)"),
            Option("Option 1", 1), 
            Option("Option 255", 255));

        switch ((Byte)await Response())
        {
            case 0:
                Message("0 was your option, no new dialogs were to be created and the session should have been closed.",
                    Option("Close"));
                break;
            case 1:
                Message("1 was your option.", Option("Close"));
                break;
            case 255:
                Message("255 was your option.", Option("Close"));
                break;
            default:
                Message("...");
                break;
        }
    }
}