using System;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Objects.Actors;

namespace Throne.World.Sessions
{
    public sealed class NpcSession
    {
        public NpcScript State { get; set; }

        /// <summary>
        ///     Create a new instance of the NPC's script and begin interaction with the user.
        /// </summary>
        /// <param name="withNpc"></param>
        public void Start(Npc withNpc, Character chr)
        {
            State = withNpc.Script.Clone(chr);
            State.Interact();
        }

        /// <summary>
        ///     Reset the NPC session. Should free related memory.
        /// </summary>
        public void Clear()
        {
            if (State)
                State.EndInteraction();
            State = null;
        }

        public Boolean Valid()
        {
            return
                State != null &&
                State.Character.CanSee(State.Npc) &&
                State.InteractionState != NpcScript.InteractionStates.Ended;
        }
    }
}