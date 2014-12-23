using System;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Objects.Actors;

namespace Throne.World.Sessions
{
    /// <summary>
    ///     An NPC interaction session.
    /// </summary>
    public sealed class NpcSession
    {
        public NpcScript State { get; set; }

        /// <summary>
        ///     Start a new NPC session
        /// </summary>
        /// <param name="withNpc"></param>
        public void Start(Npc withNpc, Character chr)
        {
            if (State) Clear();

            State = withNpc.Script.Clone(chr);

            State.InteractAsync();
        }

        /// <summary>
        ///     Reset the NPC session. Should free related memory.
        /// </summary>
        public void Clear()
        {
            State.End();
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