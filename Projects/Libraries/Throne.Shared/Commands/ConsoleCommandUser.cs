using System;
using Throne.Framework.Security;
using Throne.Framework.Security.Permissions;

namespace Throne.Framework.Commands
{
    internal sealed class ConsoleCommandUser : RestrictedObject, ICommandUser
    {
        public ConsoleCommandUser()
        {
            AddPermission(new ConsolePermission());
            AddPermission(new RootPermission());
        }

        public void Respond(string response)
        {
            Console.WriteLine(response);
        }

        public void Respond(byte[] response)
        {
        }
    }
}