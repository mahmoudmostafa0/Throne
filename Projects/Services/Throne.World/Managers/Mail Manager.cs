using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.World.Database.Records;
using Throne.World.Database.Records.Implementations;
using Throne.World.Structures.Objects;

namespace Throne.World.Managers
{
    public sealed class MailManager : SingletonActor<MailManager>
    {
        private readonly LogProxy _log;
        private readonly SerialGenerator _serialGenerator;

        private MailManager()
        {
            _log = new LogProxy("MailManager");

            SerialGeneratorManager.Instance.GetGenerator(typeof (MailRecord).Name, WorldObject.ItemIdMin,
                WorldObject.ItemIdMax, ref _serialGenerator);
        }
    }
}