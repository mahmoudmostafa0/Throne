using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;
using Throne.World.Structures.Objects.Actors;
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
        private Dictionary<UInt32, Npc> _currentVisibleNpcs;

        public void ClearScreen()
        {
            _currentVisibleCharacters.Clear();
            _currentVisibleMapItems.Clear();
        }

        public Boolean CanSee(IWorldObject obj)
        {
            return _currentVisibleCharacters.ContainsKey(obj.ID) ||
                   _currentVisibleMapItems.ContainsKey(obj.ID) ||
                   _currentVisibleNpcs.ContainsKey(obj.ID);
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
                nc.User.PostAsync(() => nc.AddVisibleCharacter(this, false));
            }
            foreach (Character rc in removeVisibleCharacters)
                rc.User.PostAsync(() => rc.RemoveVisibleCharacter(this));

            #endregion

            #region Update map items

            List<Item> updatedMapItems = map.GetVisibleItems(this);
            IEnumerable<Item> newVisibleMapItems = updatedMapItems.Except(_currentVisibleMapItems.Values);
            IEnumerable<Item> removeVisibleMapItems = _currentVisibleMapItems.Values.Except(updatedMapItems);

            _currentVisibleMapItems = updatedMapItems.ToDictionary(mi => mi.ID);

            foreach (Item ni in newVisibleMapItems)
            {
                ni.SpawnFor(User);
                ni.Script.OnEnterScreen(this, ni);
            }

            foreach (Item ni in removeVisibleMapItems)
                ni.Script.OnExitScreen(this, ni);

            #endregion

            #region Update npcs

            List<Npc> updatedNpcs = map.GetVisibleNpcs(this);
            IEnumerable<Npc> newVisibleNpcs = updatedNpcs.Except(_currentVisibleNpcs.Values);
            IEnumerable<Npc> removeVisibleNpcs = _currentVisibleNpcs.Values.Except(updatedNpcs);

            _currentVisibleNpcs = updatedNpcs.ToDictionary(n => n.ID);

            foreach (Npc nn in newVisibleNpcs)
                nn.SpawnFor(User);

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
                nc.User.PostAsync(() => nc.AddVisibleCharacter(this));
            foreach (Character rc in removeVisibleCharacters)
                rc.User.PostAsync(() => rc.RemoveVisibleCharacter(this));

            #endregion

            #region Update map items

            List<Item> updatedMapItems = map.GetVisibleItems(this);
            IEnumerable<Item> newVisibleMapItems = updatedMapItems.Except(_currentVisibleMapItems.Values);
            IEnumerable<Item> removeVisibleMapItems = _currentVisibleMapItems.Values.Except(updatedMapItems);

            _currentVisibleMapItems = updatedMapItems.ToDictionary(mi => mi.ID);

            foreach (Item ni in newVisibleMapItems)
            {
                ni.SpawnFor(User);
                ni.Script.OnEnterScreen(this, ni);
            }

            foreach (Item ni in removeVisibleMapItems)
                ni.Script.OnExitScreen(this, ni);

            #endregion

            #region Update npcs

            List<Npc> updatedNpcs = map.GetVisibleNpcs(this);
            IEnumerable<Npc> newVisibleNpcs = updatedNpcs.Except(_currentVisibleNpcs.Values);
            IEnumerable<Npc> removeVisibleNpcs = _currentVisibleNpcs.Values.Except(updatedNpcs);

            _currentVisibleNpcs = updatedNpcs.ToDictionary(n => n.ID);

            foreach (Npc nn in newVisibleNpcs)
                nn.SpawnFor(User);

            #endregion
        }

        public void EnterRegion(Location location)
        {
            Location = location;
            Location.Map.AddUser(this);

            User.SendArrays(
                new GeneralAction(ActionType.MapEnvironmentColor, this)
                {
                    Argument = Location.Map.EnvironmentColor.ToArgb()
                },
                new WeatherInformation(WeatherInformation.WeatherType.Fine, 14, 0, Color.Black),
                new MapInfo(Location.Map));

            if (LoggedIn)
                LookAround();
        }

        public void ExitCurrentRegion()
        {
            Location.Map.RemoveUser(this);
            foreach (Character rc in _currentVisibleCharacters.Values)
                rc.User.PostAsync(() => rc.RemoveVisibleCharacter(this, true));

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

        private void AddVisibleMapItem(Item itm)
        {
            itm.SpawnFor(User);
            _currentVisibleMapItems[itm.ID] = itm;
        }

        private void RemoveVisibleMapItem(Item itm, Boolean force = false)
        {
            if (force)
                itm.DespawnFor(User);

            _currentVisibleMapItems.Remove(itm.ID);
        }

        private Item GetVisibleMapItem(UInt32 Id)
        {
            return _currentVisibleMapItems[Id];
        }

        #endregion
    }
}