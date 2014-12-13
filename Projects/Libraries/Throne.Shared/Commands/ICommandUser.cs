using Throne.Shared.Security;

namespace Throne.Shared.Commands
{
    public interface ICommandUser : IPermissible
    {
        void Respond(string response);
    }
}