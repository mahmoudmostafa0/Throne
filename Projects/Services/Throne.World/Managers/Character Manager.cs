using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Throne.Framework;
using Throne.Framework.Logging;
using Throne.Framework.Network.Connectivity;
using Throne.Framework.Threading;
using Throne.World.Database.Records.Implementations;
using Throne.World.Network;
using Throne.World.Properties.Settings;
using Throne.World.Records;
using Throne.World.Structures.Objects;

namespace Throne.World
{
    public sealed class CharacterManager : SingletonActor<CharacterManager>
    {
        private readonly LogProxy _log;
        private SerialGenerator serialGenerator;

        private CharacterManager()
        {
            _log = new LogProxy("CharacterManager");

            SerialGeneratorManager.Instance.PostAsync(
                sg =>
                    serialGenerator =
                        sg.GetSerialGenerator(typeof(CharacterRecord).Name, WorldObject.PlayerIdMin,
                            WorldObject.PlayerIdMax));
        }

        public void CreateCharacter(IClient client, String name, Byte job, String macAddr, Int16 look)
        {
            var record = new CharacterRecord
            {
                Guid = serialGenerator.Next(),
                OwnerGuid = client.UserData.UserGuid, //dynamic
                Name = name,
                CurrentJob = job,
                Level = 1,
                Look = look,
                MapID = EntitySettings.Default.NewCharacterMap,
                X = (short)EntitySettings.Default.NewCharacterSpawn.X,
                Y = (short)EntitySettings.Default.NewCharacterSpawn.Y,
                CreationTime = DateTime.Now,
                CreatorMacAddress = macAddr,
                ItemPayload = new List<ItemRecord>()
            };
            record.Create();
        }

        /// <summary>
        ///     Finds a character in the database by the owner's account <code>Guid</code>
        /// </summary>
        /// <param name="client">The requesting client.</param>
        /// <returns>The character record requested.</returns>
        public CharacterRecord FindCharacterRecord(IClient client)
        {
            return WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(
                c => c.OwnerGuid == client.UserData.UserGuid).FirstOrDefault();
        }

        public Character InitiaizeCharacter(IClient client, CharacterRecord record)
        {
            return new Character((WorldClient)client, record);
        }

        public Character FindCharacter(UInt32 uid)
        {
            return new Character(null,
                WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(c => c.Guid == uid).SingleOrDefault());
        }

        public Character FindCharacter(String name)
        {
            var ingame = WorldManager.Instance.GetCharacter(name);
            if (ingame)
                return ingame;

            var inTheForest =
                WorldServer.Instance.WorldDbContext.Find<CharacterRecord>(c => c.Name == name).SingleOrDefault();
            return inTheForest ? new Character(null, inTheForest) : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean NameValid(String name, Boolean checkDb = false)
        {
            var valid =
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