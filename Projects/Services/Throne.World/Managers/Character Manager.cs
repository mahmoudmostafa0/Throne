using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Throne.Framework;
using Throne.Framework.Logging;
using Throne.Framework.Threading;
using Throne.World.Database.Records.Implementations;
using Throne.World.Network;
using Throne.World.Records;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;

namespace Throne.World
{
    public sealed class CharacterManager : SingletonActor<CharacterManager>
    {
        private readonly LogProxy _log;
        private readonly SerialGenerator _serialGenerator;

        private CharacterManager()
        {
            _log = new LogProxy("CharacterManager");

            SerialGeneratorManager.Instance.GetGenerator(typeof (CharacterRecord).Name, WorldObject.PlayerIdMin,
                WorldObject.PlayerIdMax, ref _serialGenerator);
        }

        public Boolean CreateCharacter(WorldClient client, String name, Role.Profession job, String macAddr, Int16 look)
        {
            var record = new CharacterRecord
            {
                Guid = _serialGenerator.Next(),
                OwnerGuid = client.AccountData.Guid,
                Name = name,
                CurrentJob = job,
                Level = 1,
                Look = look,
                CreationTime = DateTime.Now,
                CreatorMacAddress = macAddr
            };

            DynamicScript.ExecutionResult rtn =
                ScriptManager.Instance.GetDynamicScript("new_character").TryExecution(record);
            if (rtn == DynamicScript.ExecutionResult.Success) return true;

            _log.Error("Character creation script failed. Return: {0}", rtn);
            return false;
        }

        /// <summary>
        ///     Finds a character in the database by the owner's account <code>Guid</code>
        /// </summary>
        /// <param name="client">The requesting client.</param>
        /// <returns>The character record requested.</returns>
        public CharacterRecord FindCharacterRecord(WorldClient client)
        {
            return WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(
                c => c.OwnerGuid == client.AccountData.Guid).FirstOrDefault();
        }

        public Character InitiaizeCharacter(WorldClient client, CharacterRecord record)
        {
            return new Character(client, record);
        }

        public Character FindCharacter(UInt32 uid)
        {
            return new Character(null,
                WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(c => c.Guid == uid).SingleOrDefault());
        }

        public Character FindCharacter(String name)
        {
            Character ingame = WorldManager.Instance.GetCharacter(name);
            if (ingame)
                return ingame;

            CharacterRecord inTheForest =
                WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(c => c.Name == name).SingleOrDefault();
            return inTheForest ? new Character(null, inTheForest) : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean NameValid(String name, Boolean checkDb = false)
        {
            bool valid =
                name.Length <= 16 &&
                name.Length >= 3 &&
                name.All(c => c > ' ' && c <= '~') &&
                !name.Contains("GM", StringComparison.OrdinalIgnoreCase) &&
                !name.Contains("PM", StringComparison.OrdinalIgnoreCase) &&
                !name.Contains("SYSTEM", StringComparison.OrdinalIgnoreCase) &&
                !name.Contains("{") &&
                !name.Contains("}") &&
                !name.Contains("[") &&
                !name.Contains("]");

            if (valid && checkDb)
                valid =
                    WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(c => c.Name == name)
                        .SingleOrDefault() != null;

            return valid;
        }
    }
}