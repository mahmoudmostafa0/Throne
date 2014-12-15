using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Travel;
using Throne.World.Structures.World;

namespace Throne.World.Structures.Objects
{
    /// <summary>
    ///     What a user can see.
    /// </summary>
    partial class Character
    {
        private readonly ReaderWriterLockSlim _scrChrRWLS;

        private Dictionary<UInt32, Character> _currentVisibleCharacters;

        public void ClearScreen()
        {
            _scrChrRWLS.EnterWriteLock();
            try
            {
                _currentVisibleCharacters.Clear();
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }
        }

        public Boolean Visible(IWorldObject obj)
        {
            return Location.Position.InRange(obj.Location.Position, EntitySettings.Default.ScreenRange);
        }


        public void LookDown(Jump jmp)
        {
            Map map = Location.Map;

            #region Update characters

            List<Character> updatedCharacters = map.GetVisibleUsers(this);
            IEnumerable<Character> newVisibleCharacters = updatedCharacters.Except(_currentVisibleCharacters.Values);
            IEnumerable<Character> removeVisibleCharacters = _currentVisibleCharacters.Values.Except(updatedCharacters);
            _scrChrRWLS.EnterWriteLock();
            try
            {
                _currentVisibleCharacters = updatedCharacters.ToDictionary(c => c.ID);
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }
            Parallel.ForEach(newVisibleCharacters, chr =>
            {
                ExchangeAerialSpawns(chr, jmp);
                AddVisibleCharacter(this, spawn: false);
            });
            Parallel.ForEach(removeVisibleCharacters, rc => rc.RemoveVisibleCharacter(this));

            User.Send(Location.Map.GetCell(Location.Position).ToString());
            #endregion
        }

        public void LookAround()
        {
            Map map = Location.Map;

            #region Update characters

            List<Character> updatedCharacters = map.GetVisibleUsers(this);
            IEnumerable<Character> newVisibleCharacters = updatedCharacters.Except(_currentVisibleCharacters.Values);
            IEnumerable<Character> removeVisibleCharacters = _currentVisibleCharacters.Values.Except(updatedCharacters);
            _scrChrRWLS.EnterWriteLock();
            try
            {
                _currentVisibleCharacters = updatedCharacters.ToDictionary(c => c.ID);
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }
            Parallel.ForEach(newVisibleCharacters, nc => nc.AddVisibleCharacter(this));
            Parallel.ForEach(removeVisibleCharacters, rc => rc.RemoveVisibleCharacter(this));

            User.Send(Location.Map.GetCell(Location.Position).ToString());

            #endregion
        }


        private void AddVisibleCharacter(Character chr, Boolean spawn = true)
        {
            if (spawn)
                ExchangeSpawns(chr);
            _scrChrRWLS.EnterWriteLock();
            try
            {
                _currentVisibleCharacters[chr.ID] = chr;
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }
        }

        private void RemoveVisibleCharacter(Character chr, Boolean force = false)
        {
            if (force)
                using (var removeEntity = new GeneralAction(ActionType.RemoveEntity, chr))
                    User.Send(removeEntity);

            _scrChrRWLS.EnterWriteLock();
            try
            {
                _currentVisibleCharacters.Remove(chr.ID);
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }
        }

        public Character GetVisibleCharacter(UInt32 Id)
        {
            _scrChrRWLS.EnterReadLock();
            try
            {
                return _currentVisibleCharacters[Id];
            }
            finally
            {
                _scrChrRWLS.ExitReadLock();
            }
        }


        public void EnterRegion(Location location)
        {
            Location = location;
            Location.Map.AddUser(this);
            using (var pkt = new MapInfo(Location.Map)) User.Send(pkt);
            LookAround();
        }

        public void ExitCurrentRegion()
        {
            _scrChrRWLS.EnterWriteLock();
            try
            {
                Location.Map.RemoveUser(this);
                Parallel.ForEach(_currentVisibleCharacters.Values, nc => nc.RemoveVisibleCharacter(this, true));
            }
            finally
            {
                _scrChrRWLS.ExitWriteLock();
            }

            ClearScreen();
        }

        #region Movement

        public Boolean Jump(Jump jmp)
        {
            int sDistance = Location.Position.GetDistance(jmp.Destination);
            int cDistance = jmp.ReportedCurrentPosition.GetDistance(jmp.Destination);

            if (sDistance > EntitySettings.Default.MaxJumpDistance)
                return false;
            if (sDistance != cDistance)
                Log.Warn(StrRes.SMSG_SynchroLost);

            Direction = Location.Position.GetOrientation(jmp.Destination);
            Location.Position.Relocate(jmp.Destination);
            SendToLocal(jmp.Info, true);

            if (!Location)
            {
                Location.Position.Restore();
                Logout();
                return false;
            }

            LookDown(jmp);

            return true;
        }

        public Boolean GroundMovement(GroundMovement gMov)
        {
            Location _new = Location;
            Position destination = _new.Position.Slide(gMov.Orientation);

            if (!_new)
            {
                Logout();
                return false;
            }

            SendToLocal(gMov, true);
            Direction = Location.Position.GetOrientation(destination);
            Location.Position.Relocate(destination);
            LookAround();

            return true;
        }

        /// TODO: Cancel and reset actions like auto attack
        /// TODO: validate movement timestamps
        /// TODO: Check for fast movements with current timestamps.
        /// TODO: remove invulnerability status

        #endregion
    }
}