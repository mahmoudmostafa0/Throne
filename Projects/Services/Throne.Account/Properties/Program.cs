using System;
using Throne.Framework.Commands;
using Throne.Framework.Threading.Actors;

namespace Throne.Login
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (ActorContext.Global)
            {
                AuthServer.Instance.Start(args);
                CommandConsole.CatchInput();
                AuthServer.Instance.Stop();
            }
        }
    }
}