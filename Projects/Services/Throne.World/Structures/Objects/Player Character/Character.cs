using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Throne.Framework;
using Throne.Framework.Collections;
using Throne.Framework.Logging;
using Throne.Framework.Runtime;
using Throne.World.Network;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Records;
using Throne.World.Sessions;
using Throne.World.Structures.Mail;
using Throne.World.Structures.Objects.Actors;
using Throne.World.Structures.Storage;
using Throne.World.Structures.Travel;
using Constants = Throne.World.Properties.Constants;

namespace Throne.World.Structures.Objects
{
    /// <summary> An in-game user controlled entity with an archetype. </summary>
    public sealed partial class Character : Role, IDisposableResource
    {
        public readonly SemaphoreSlim InitializationSignal;

        /// <summary>
        ///     Initializes a new character object.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="record"></param>
        public Character(WorldClient user, CharacterRecord record)
            : base(record.Guid)
        {
            NpcSession = new NpcSession();

            User = user;
            Record = record;

            User.Log = new LogProxy("{0}:{1}".Interpolate(Name, user.ClientAddress));
            Log.Info(StrRes.SMSG_LoggedIn);

            _timers = new Dictionary<CharacterTask, CharacterTimer>();

            _look = new Model(Record.Look);
            _pStats = new BooleanArray<RoleState>(192);

            IEnumerable<Item> items = Record.ItemPayload.Select(itemRecord => new Item(itemRecord));
            {
                _gear = new Gear(items);
                _inventory = new Inventory(items);
            }

            Inbox = new Inbox(Record.MailPayload.Select(mailRecord => new Mail.Mail(mailRecord)));

            _currentVisibleCharacters = new Dictionary<UInt32, Character>();
            _currentVisibleMapItems = new Dictionary<UInt32, Item>();
            _currentVisibleNpcs = new Dictionary<UInt32, Npc>();

            InitializationSignal = new SemaphoreSlim(0, 1);
            Initialize();
        }

        public LogProxy Log
        {
            get { return User.Log; }
        }

        public void Dispose()
        {
            if (Location)
                ExitCurrentRegion();

            ClearScreen();
            NpcSession.Clear();

            Log.Info(StrRes.SMSG_LoggedOut);
        }

        public bool IsDisposed { get; private set; }

        public async void Initialize()
        {
            User.PostAsync(() => Send(new LoadMap(Record.MapID)));

            try
            {
                await InitializationSignal.WaitAsync(5000);
            }
            catch (OperationCanceledException)
            {
                Log.Error("Map load signal not received from client.");
                Logout();
                return;
            }

            User.SendArrays(
                new CharacterInformation(this)
                //mentor
                //goodwill ranks
                //guild
                //title
                );

            EnterRegion(new Location(Record.MapID, Record.X, Record.Y));
        }

        public void Save()
        {
            if (Location)
            {
                Record.X = Location.Position.X;
                Record.Y = Location.Position.Y;
                Record.MapID = Location.MapId;
                Record.InstanceId = Location.Map.Instance;
            }

            Record.Update();
        }

        public void Logout()
        {
            User.Disconnect();
        }


        public override string ToString()
        {
            return Name;
        }

        #region Sending

        public void SendToLocal(WorldPacket packet = null, Boolean includeSelf = false)
        {
            if (!packet) packet = this;
            if (includeSelf) User.Send(packet);

            foreach (Character user in _currentVisibleCharacters.Values)
                user.User.Send(packet);
        }

        public override void SpawnFor(WorldClient observer)
        {
            ExchangeSpawns(observer.Character);
        }

        public override void DespawnFor(WorldClient observer)
        {
            using (var pkt = new GeneralAction(ActionType.RemoveEntity, this))
                observer.Send(pkt);
        }

        public void Send(WorldPacket packet)
        {
            User.Send(packet);
        }

        public void ExchangeSpawns(Character with)
        {
            //TODO: goodwill suits

            with.User.Send((RoleInfo) this);
            User.Send((RoleInfo) with);
        }

        /// <summary>
        ///     Sends this entity's spawn at the edge of the other character's screen, then sends a jump.
        /// </summary>
        /// <param name="with"></param>
        /// <param name="jmp"></param>
        /// <remarks>
        ///     TODO:
        ///     I'm clearly rushing here. My method for entering into screens gracefully is not proper.
        ///     The position of this entity is relocated, causing other players to exchange invalid spawn info or
        ///     to interact with the player at the old location.
        /// </remarks>
        public void ExchangeAerialSpawns(Character with, Jump jmp)
        {
            Position pos = Location.Position.GetPrevious();
            Position otherPos = with.Location.Position;
            int pDistance = otherPos - pos;
            int gap = MapSettings.Default.PlayerScreenRange - pDistance;
            Position fauxPos = otherPos.GetRelative(pos, gap);

            Location.Position.Relocate(fauxPos);
            ExchangeSpawns(with);
            Location.Position.Restore();

            with.User.Send(jmp.Info);
        }

        public static implicit operator RoleInfo(Character characterToSpawn)
        {
            return new RoleInfo(characterToSpawn);
        }

        #endregion
    }
}