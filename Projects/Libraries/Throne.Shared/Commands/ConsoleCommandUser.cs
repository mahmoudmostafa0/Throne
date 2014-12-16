using System;
using Throne.Shared.Security;
using Throne.Shared.Security.Permissions;

namespace Throne.Shared.Commands
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
            throw new NotImplementedException();
        }
    }
}