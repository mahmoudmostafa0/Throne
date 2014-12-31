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
        SetLongName("Twin City Guild Director");
        SetFacing(Orientation.Southwest);
        SetType(NpcInformation.Types.Talker);
        SetLocation(new Location(1002, new Position(269, 291)));
    }

    protected override async Task Talk()
    {
        start:
        Message("Hello {0}, this is Throne's first interactive dialog.".Interpolate(Character.Name),
            Option("Option 0 (Close)"),
            Option("Option 1", 1),
            Option("Input", 2),
            Option("Option 255", 255));

        switch ((await Response()).Option)
        {
            case 0:
                Message("0 was your option, no new dialogs were to be created and the session should have been closed.",
                    Option("Close"));
                break;
            case 1:
                Message("1 was your option.", Option("Close"));
                break;
            case 2:
                Message("Input test...\nType test to get an alternate message",
                    Input("Test", 1, 255),
                    Option("Go Back", 2));

                await Response();

                if (Feedback == "test")
                {
                    Message("You wrote test.");
                    break;
                }

                if (Feedback == 2)
                    goto start;

                Message("Input Result\n________________\n\nOption:{0}\nText:{1}\n\n".Interpolate(Feedback.Option,
                    Feedback.Input));
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