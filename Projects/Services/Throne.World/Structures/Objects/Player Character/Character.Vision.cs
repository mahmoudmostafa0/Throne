using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Travel;
using Throne.World.Structures.World;

namespace Throne.World.Structures.Objects
{
    /// <summary>
    ///     What a user can see.
    /// </summary>
    /// <remarks>
    ///     This part should be kept thread-safe.
    ///     If "this" entity tries to remove a value from another entity it should posted as a message to the other
    ///     entity's actor, since each user's messages execute on only one thread no matter what.
    /// </remarks>
    partial class Character
    {
        private Dictionary<UInt32, Character> _currentVisibleCharacters;
        private Dictionary<UInt32, Item> _currentVisibleMapItems; // using the item class for now

        public void ClearScreen()
        {
            _currentVisibleCharacters.Clear();
            _currentVisibleMapItems.Clear();
        }

        public Boolean IsVisible(IWorldObject obj)
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

            _currentVisibleCharacters = updatedCharacters.ToDictionary(c => c.ID);

            foreach (Character nc in newVisibleCharacters)
            {
                ExchangeAerialSpawns(nc, jmp);
                nc.User.PostAsync(() => AddVisibleCharacter(this, false));
            }
            foreach (Character rc in removeVisibleCharacters)
                rc.User.PostAsync(() => RemoveVisibleCharacter(this));

            #endregion

            #region Update map items

            List<Item> updatedMapItems = map.GetVisibleItems(this);
            IEnumerable<Item> newVisibleMapItems = updatedMapItems.Except(_currentVisibleMapItems.Values);
            IEnumerable<Item> removeVisibleMapItems = _currentVisibleMapItems.Values.Except(updatedMapItems);

            _currentVisibleMapItems = updatedMapItems.ToDictionary(mi => mi.ID);

            foreach (Item ni in newVisibleMapItems)
                ni.SpawnFor(User);
            //TODO: Continue

            #endregion
        }

        public void LookAround()
        {
            Map map = Location.Map;

            #region Update characters

            List<Character> updatedCharacters = map.GetVisibleUsers(this);
            IEnumerable<Character> newVisibleCharacters = updatedCharacters.Except(_currentVisibleCharacters.Values);
            IEnumerable<Character> removeVisibleCharacters = _currentVisibleCharacters.Values.Except(updatedCharacters);

            _currentVisibleCharacters = updatedCharacters.ToDictionary(c => c.ID);

            foreach (Character nc in newVisibleCharacters)
                nc.User.PostAsync(() => AddVisibleCharacter(this));
            foreach (Character rc in removeVisibleCharacters)
                rc.User.PostAsync(() => RemoveVisibleCharacter(this));

            #endregion
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
            Location.Map.RemoveUser(this);
            foreach (Character rc in _currentVisibleCharacters.Values)
                rc.User.PostAsync(() => RemoveVisibleCharacter(this, true));

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

        #region Characters
        private void AddVisibleCharacter(Character chr, Boolean spawn = true)
        {
            if (spawn)
                ExchangeSpawns(chr);

            _currentVisibleCharacters[chr.ID] = chr;
        }

        private void RemoveVisibleCharacter(Character chr, Boolean force = false)
        {
            if (force)
                chr.DespawnFor(User);

            _currentVisibleCharacters.Remove(chr.ID);
        }

        public Character GetVisibleCharacter(UInt32 Id)
        {
            return _currentVisibleCharacters[Id];
        }

        #endregion

        #region Map Items

        #endregion
    }
}