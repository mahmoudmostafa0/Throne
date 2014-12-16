using System;
using System.Collections.Generic;
using System.Diagnostics;
using Throne.Shared;
using Throne.Shared.Threading.Actors;
using Throne.World.Properties.Settings;

namespace Throne.World.Structures.Objects
{
    /// <summary>  This part includes tasks to be run on a character. </summary>
    partial class Character
    {
        private readonly Object _syncTimerDictionary = new Object();
        private readonly Dictionary<CharacterTask, CharacterTimer> _timers;
        private Boolean _canBeginTasks = true;

        /// <summary>
        ///     Creates a new <see cref="Throne.Shared.Threading.Actors.ActorTimer" />
        ///     A timer is used to invoke tasks on the current thread of this instance's
        ///     <see cref="Throne.World.Network.WorldClient" />
        ///     The action is passed to the actor's current thread for the sake of load balancing.
        /// </summary>
        /// <param name="action">What is your task, oh great one? (use lambada expressions to do anything at all)</param>
        /// <param name="wait">When should your task be started?</param>
        /// <param name="interval">After your task is started, how long before it should be attempted again?</param>
        /// <param name="task">And just what kind of task do you think this is? (For meddling with the timer)</param>
        public void BeginWaitTask(Action action, TimeSpan wait, TimeSpan interval, CharacterTask task)
        {
#if DEBUG
            Debug.Assert(action != null);
            Debug.Assert(User != null);
#endif
            if (!_canBeginTasks)
            {
                Log.Warn(StrRes.SMSG_CharacterObjectNotReadyForTasks, task);
                return;
            }

            lock (_syncTimerDictionary)
            {
                if (_timers.ContainsKey(task))
                {
                    Log.Warn(StrRes.SMSG_RunningTaskOverride, task);
                    EndTask(task);
                }
                _timers.Add(task, new CharacterTimer(task, User, action, wait, interval));
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Throne.Shared.Threading.Actors.ActorTimer" />
        ///     A timer is used to invoke tasks on the thread this character's actor picks.
        /// </summary>
        /// <param name="action">What is your task, oh great one? (use lambada expressions to do anything at all)</param>
        /// <param name="interval">After your task is started, how long before it should be attempted again?</param>
        /// <param name="task">And just what kind of task do you think this is? (For meddling with the timer)</param>
        public void BeginTask(Action action, TimeSpan interval, CharacterTask task)
        {
            BeginWaitTask(action, TimeSpan.Zero, interval, task);
        }

        /// <summary>
        ///     This will change the interval at which an active task is executed.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="executionInterval"></param>
        public Boolean ChangeTaskDelay(CharacterTask task, TimeSpan executionInterval)
        {
            CharacterTimer tmr;
            lock (_syncTimerDictionary)
            {
                if (!_timers.TryGetValue(task, out tmr)) return false;

                tmr.Change(TimeSpan.Zero, executionInterval);

                return true;
            }
        }

        /// <summary>
        ///     Dispose that task! (if it didn't dispose itself on it's own action)
        /// </summary>
        /// <param name="task"></param>
        public Boolean EndTask(CharacterTask task)
        {
            CharacterTimer tmr;
            lock (_syncTimerDictionary)
            {
                if (!_timers.TryGetValue(task, out tmr))
                    return false;
                _timers.Remove(task);
                tmr.Dispose();

                return true;
            }
        }
    }

    public enum CharacterTask
    {
        AutoSave,

        Count
    }

    internal class CharacterTimer : ActorTimer
    {
        public CharacterTask Task;

        public CharacterTimer(CharacterTask task, IActor target, Action callback, TimeSpan delay, TimeSpan period)
            : base(target, callback, delay, period)
        {
            Task = task;
        }

        public static implicit operator CharacterTask(CharacterTimer ct)
        {
            return ct.Task;
        }

        public override string ToString()
        {
            return StrRes.SMSG_CharacterTimerString.Interpolate(Task);
        }

        protected override void TimerCallback(object state)
        {

            Console.WriteLine(this.Callback.GetType().Name);
            base.TimerCallback(state);
        }
    }
}