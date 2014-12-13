using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.World.Network;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.World
{
    /// <summary>
    ///     Map operations for user characters.
    /// </summary>
    public partial class Map
    {
        private readonly Dictionary<UInt32, Character> _users;
        private Object _usersRWL;

        public Object UserReadWrite
        {
            get
            {
                if (_usersRWL == null) Interlocked.CompareExchange<Object>(ref _usersRWL, new Object(), null);
                return _usersRWL;
            }
        }

        public void AddUser(Character chr)
        {
            lock (UserReadWrite)
                _users[chr.ID] = chr;

            Script.OnEnter(chr);
        }

        public void RemoveUser(Character chr)
        {
            Script.OnExit(chr);

            lock (UserReadWrite)
                _users.Remove(chr.ID);
        }

        public Character GetUser(String name)
        {
            lock (UserReadWrite)
                return _users.Values.FirstOrDefault(a => a.Name == name);
        }

        public List<Character> GetVisibleUsers(IWorldObject nearThis)
        {
            var result = new List<Character>();
            Position pos = nearThis.Location.Position;

            lock (UserReadWrite)
                result.AddRange(
                    _users.Values.Where(
                        usr =>
                            usr.ID != nearThis.ID &&
                            usr.Location.Position.InRange(pos, MapSettings.Default.PlayerScreenRange)));
            return result;
        }

        public void Send(WorldPacket packet)
        {
            lock (UserReadWrite)
                foreach (Character user in _users.Values.Where(u => u != null).ToArray())
                    user.Send(packet);
        }
    }
}