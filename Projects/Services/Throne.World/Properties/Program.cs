using Throne.Shared.Commands;
using Throne.Shared.Threading.Actors;

namespace Throne.World
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (ActorContext.Global)
            {
                WorldServer.Instance.Start(args);
                CommandConsole.CatchInput();
                WorldServer.Instance.Stop();
            }
        }
    }
}