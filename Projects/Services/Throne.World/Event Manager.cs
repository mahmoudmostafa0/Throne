using System;
using Throne.Shared;
using Throne.World.Security;
using Throne.World.Structures.Objects;

namespace Throne.World
{
    public class EventManager
    {
        /// <summary>
        /// Raised when there's a security violation
        /// </summary>
        public event Action<SecurityViolationEventArgs> SecurityViolation;
        public void OnSecurityViolation(SecurityViolationEventArgs args) { SecurityViolation.Raise(args); }

        // ------------------------------------------------------------------

        /// <summary>
        /// Raised a few seconds after player logged in.
        /// </summary>
        public event Action<Creature> PlayerLoggedIn;
        public void OnPlayerLoggedIn(Creature creature) { PlayerLoggedIn.Raise(creature); }

        /// <summary>
        /// Raised when a player disconnects from server.
        /// </summary>
        public event Action<Creature> PlayerDisconnect;
        public void OnPlayerDisconnect(Creature creature) { PlayerDisconnect.Raise(creature); }

        /// <summary>
        /// Raised when player enters a region.
        /// </summary>
        public event Action<Creature> PlayerEntersRegion;
        public void OnPlayerEntersRegion(Creature creature) { PlayerEntersRegion.Raise(creature); }

        /// <summary>
        /// Raised when player leaves a region.
        /// </summary>
        public event Action<Creature> PlayerLeavesRegion;
        public void OnPlayerLeavesRegion(Creature creature) { PlayerLeavesRegion.Raise(creature); }

        /// <summary>
        /// Raised when player drops, destroys, sells,
        /// uses (decrements), etcs an item.
        /// </summary>
        public event Action<Creature, int, int> PlayerRemovesItem;
        public void OnPlayerRemovesItem(Creature creature, int itemId, int amount) { PlayerRemovesItem.Raise(creature, itemId, amount); }

        /// <summary>
        /// Raised when player receives an item in any way.
        /// </summary>
        public event Action<Creature, int, int> PlayerReceivesItem;
        public void OnPlayerReceivesItem(Creature creature, int itemId, int amount) { PlayerReceivesItem.Raise(creature, itemId, amount); }

        /// <summary>
        /// Raised when a creature is killed by something.
        /// </summary>
        public event Action<Creature, Creature> CreatureKilled;
        public void OnCreatureKilled(Creature creature, Creature killer) { CreatureKilled.Raise(creature, killer); }

        /// <summary>
        /// Raised when a creature is killed by a player.
        /// </summary>
        public event Action<Creature, Creature> CreatureKilledByPlayer;
        public void OnCreatureKilledByPlayer(Creature creature, Creature killer) { CreatureKilledByPlayer.Raise(creature, killer); }
    }

    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Raises event with thread and null-ref safety.
        /// </summary>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }
    }
}
