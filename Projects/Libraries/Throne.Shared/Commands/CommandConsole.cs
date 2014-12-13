using System;

namespace Throne.Shared.Commands
{
    public static class CommandConsole
    {
        private static readonly ConsoleCommandUser _user = new ConsoleCommandUser();

        public static bool StopConsole { get; set; }

        public static void CatchInput()
        {
            while (!StopConsole)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;

                CommandManager.Instance.ExecuteCommand(new CommandArguments(line.ParseCommand()), _user);
            }
        }
    }
}