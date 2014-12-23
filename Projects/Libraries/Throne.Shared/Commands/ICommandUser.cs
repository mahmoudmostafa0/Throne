using System;
using Throne.Framework.Security;

namespace Throne.Framework.Commands
{
    public interface ICommandUser : IPermissible
    {
        void Respond(String response);
        void Respond(Byte[] response);
    }
}